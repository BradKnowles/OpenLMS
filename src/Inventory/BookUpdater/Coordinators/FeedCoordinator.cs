using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;

using CodeHollow.FeedReader;

using Zio;

namespace OpenLMS.Inventory.BookUpdater.Coordinators
{
    public class FeedCoordinator : ReceiveActor
    {
        private readonly ILoggingAdapter _log;

        public FeedCoordinator(IActorRef downloadCoordinator, IActorRef fileSystemSupervisor)
        {
            if (downloadCoordinator == null) throw new ArgumentNullException(nameof(downloadCoordinator));
            if (fileSystemSupervisor == null) throw new ArgumentNullException(nameof(fileSystemSupervisor));

            _log = Context.GetLogger<SerilogLoggingAdapter>()
                .ForContext("ActorName", $"{Self.Path.Name}#{Self.Path.Uid}");

            Receive<Messages.DownloadFeed>(message =>
            {
                _log.Debug("Received {Message}", message);
                var x = new List<IActorRef>
                {
                    Self
                };
                var downloadMessage =
                    new DownloadCoordinator.Messages.DownloadUrl(message.FeedUrl,
                        message.FeedDownloadPath, x.ToImmutableArray(), message.CorrelationId, message.MessageId);

                downloadCoordinator.Tell(downloadMessage);
            });

            Receive<Messages.FeedDownloaded>(message =>
            {
                _log.Debug("Received {Message}", message);

                Byte[] feedArray = new Byte[message.Contents.Length];
                message.Contents.CopyTo(feedArray);
                Feed feed = FeedReader.ReadFromByteArray(feedArray);

                foreach (FeedItem item in feed.Items)
                {
                    String bookId = $"{item.Link.Split('/').Last()}";
                    String fileName = $"./feeds/{bookId}.rdf";
                    var url = new Uri($"{item.Link}.rdf");
                    var downloadMessage = new DownloadCoordinator.Messages.DownloadUrl(url, fileName, message.CorrelationId, message.MessageId);
                    downloadCoordinator.Tell(downloadMessage, Self);
                }
            });

            Receive<DownloadCoordinator.Messages.DownloadComplete>(message =>
            {
                _log.Debug("Received {Message}", message);

                Byte[] feedArray = new Byte[message.Contents.Length];
                message.Contents.CopyTo(feedArray);
                Feed feed = FeedReader.ReadFromByteArray(feedArray);

                foreach (FeedItem item in feed.Items)
                {
                    String bookId = $"{item.Link.Split('/').Last()}";
                    String fileName = $"./feeds/{bookId}.rdf";
                    var url = new Uri($"{item.Link}.rdf");
                    var downloadMessage = new DownloadCoordinator.Messages.DownloadUrl(url, fileName, message.CorrelationId, message.MessageId);
                    downloadCoordinator.Tell(downloadMessage, Self);
                }
            });
        }

        public static IActorRef Create(IActorRefFactory actorRefFactory, IActorRef downloadCoordinator,
            IActorRef fileSystemSupervisor, String name = null)
        {
            if (actorRefFactory == null) throw new ArgumentNullException(nameof(actorRefFactory));
            if (downloadCoordinator == null) throw new ArgumentNullException(nameof(downloadCoordinator));
            if (fileSystemSupervisor == null) throw new ArgumentNullException(nameof(fileSystemSupervisor));

            return actorRefFactory.ActorOf(Props.Create<FeedCoordinator>(downloadCoordinator, fileSystemSupervisor),
                String.IsNullOrWhiteSpace(name) ? "feedCoordinator" : name);
        }

        protected override void PreStart() => _log.Info("Actor started");
        protected override void PostStop() => _log.Info("Actor stopped");

        public static class Messages
        {
            public record DownloadFeed
            {
                public DownloadFeed(Uri feedUrl) : this(feedUrl, UPath.Empty) { }

                public DownloadFeed(Uri feedUrl, UPath feedDownloadPath)
                {
                    var id = Guid.NewGuid();
                    FeedUrl = feedUrl;
                    FeedDownloadPath = feedDownloadPath;
                    MessageId = id;
                    CorrelationId = id;
                    CausationId = id;
                }

                public Uri FeedUrl { get; init; }
                public UPath FeedDownloadPath { get; init; }
                public Boolean SaveToFile => String.IsNullOrWhiteSpace(FeedDownloadPath.FullName) == false;
                public Guid MessageId { get; init; }
                public Guid CorrelationId { get; init; }
                public Guid CausationId { get; init; }
            }

            public record FeedDownloaded
            {
                public FeedDownloaded(ImmutableArray<Byte> contents, Guid correlationId, Guid causationId)
                {
                    Contents = contents;
                    CorrelationId = correlationId;
                    CausationId = causationId;
                }

                public ImmutableArray<Byte> Contents { get; init; }
                public Guid MessageId { get; init; } = Guid.NewGuid();
                public Guid CorrelationId { get; init; }
                public Guid CausationId { get; init; }
            }

            internal record ParsedFeed(Feed Feed);
        }
    }
}
