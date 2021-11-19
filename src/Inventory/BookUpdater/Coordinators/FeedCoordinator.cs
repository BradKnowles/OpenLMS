using System;

using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;

using CodeHollow.FeedReader;

using OpenLMS.Inventory.BookUpdater.Actors;

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
                    new DownloadCoordinator.Messages.DownloadUrl(message.FeedUrl, message.CorrelationId,
                        message.MessageId);
                downloadCoordinator.Tell(downloadMessage, Self);
            });

            Receive<SharedMessages.DownloadComplete>(message =>
            {
                _log.Debug("Received {Message}", message);
                fileSystemSupervisor.Tell(new FileSystemSupervisor.Messages.SaveFile("today.rss", message.Contents,
                    message.CorrelationId, message.MessageId));

                IActorRef parseFeedActor = ParseFeedActor.Create(Context);
                parseFeedActor.Tell(new ParseFeedActor.Messages.ParseFeed(message.Contents, message.CorrelationId,
                    message.MessageId));
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
                public DownloadFeed(Uri feedUrl)
                {
                    var id = Guid.NewGuid();
                    FeedUrl = feedUrl;
                    MessageId = id;
                    CorrelationId = id;
                    CausationId = id;
                }

                public Uri FeedUrl { get; init; }
                public Guid MessageId { get; init; }
                public Guid CorrelationId { get; init; }
                public Guid CausationId { get; init; }
            }

            internal record ParsedFeed(Feed Feed);
        }
    }
}
