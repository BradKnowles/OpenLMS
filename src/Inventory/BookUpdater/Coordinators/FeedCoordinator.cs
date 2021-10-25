using System;

using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;

using CodeHollow.FeedReader;

using OpenLMS.Inventory.BookUpdater.Actors;
using OpenLMS.Inventory.BookUpdater.SharedMessages;

namespace OpenLMS.Inventory.BookUpdater.Coordinators
{
    internal class FeedCoordinator : ReceiveActor
    {
        private readonly ILoggingAdapter _log;

        public FeedCoordinator(IActorRef downloadCoordinator)
        {
            _log = Context.GetLogger<SerilogLoggingAdapter>()
                .ForContext("ActorName", $"{Self.Path.Name}#{Self.Path.Uid}");

            Receive<Messages.DownloadFeed>(message =>
            {
                _log.Debug("Received {Message}", message);
                var downloadMessage =
                    new DownloadCoordinator.Messages.DownloadUrl(message.FeedUrl, message.CorrelationId,
                        message.MessageId);
                downloadCoordinator.Tell(downloadMessage);
            });

            Receive<DownloadComplete>(message =>
            {
                _log.Debug("Received {Message}", message);
                IActorRef parseFeedActor = ParseFeedActor.Create(Context);
                parseFeedActor.Tell(new ParseFeedActor.Messages.ParseFeed(message.Contents, message.CorrelationId,
                    message.MessageId));
            });
        }


        public static IActorRef Create(IActorRefFactory actorRefFactory, IActorRef downloadCoordinator,
            String name = null) =>
            actorRefFactory.ActorOf(Props.Create<FeedCoordinator>(downloadCoordinator),
                String.IsNullOrWhiteSpace(name) ? "feedCoordinator" : name);

        protected override void PreStart() => _log.Info("Actor started");
        protected override void PostStop() => _log.Info("Actor stopped");

        internal static class Messages
        {
            internal record DownloadFeed
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
