using System;

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
                var downloadMessage =
                    new DownloadCoordinator.Messages.DownloadUrl(message.FeedUrl,
                        message.FeedDownloadPath, message.CorrelationId, message.MessageId);
                downloadCoordinator.Tell(downloadMessage, Self);
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

            internal record ParsedFeed(Feed Feed);
        }
    }
}
