using System;
using System.Collections.Immutable;
using System.Linq;

using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;

using CodeHollow.FeedReader;

using OpenLMS.Inventory.BookUpdater.Coordinators;

namespace OpenLMS.Inventory.BookUpdater.Actors
{
    internal class ParseFeedActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log;

        public ParseFeedActor()
        {
            _log = Context.GetLogger<SerilogLoggingAdapter>()
                .ForContext("ActorName", $"{Self.Path.Name}#{Self.Path.Uid}");

            Receive<Messages.ParseFeed>(message =>
            {
                _log.Debug("Received {Message}", message);
                _log.Info("Received Message {ID}", message.MessageId);

                Feed feed = FeedReader.ReadFromByteArray(message.FeedContents.ToArray());
                Console.WriteLine(feed.Description);
                Sender.Tell(new FeedCoordinator.Messages.ParsedFeed(feed));

                Context.Stop(Self);
            });
        }

        protected override void PreStart() => _log.Info("Actor started");
        protected override void PostStop() => _log.Info("Actor stopped");

        public static IActorRef Create(IActorRefFactory actorRefFactory, String name = null) =>
            actorRefFactory.ActorOf(Props.Create<ParseFeedActor>(),
                String.IsNullOrWhiteSpace(name) ? "parseFeed" : name);

        internal static class Messages
        {
            internal sealed record ParseFeed
            {
                public ParseFeed(ImmutableArray<Byte> feedContents, Guid correlationId, Guid causationId)
                {
                    FeedContents = feedContents;
                    CorrelationId = correlationId;
                    CausationId = causationId;
                }

                public ImmutableArray<Byte> FeedContents { get; init; }
                public Guid MessageId { get; init; } = Guid.NewGuid();
                public Guid CorrelationId { get; init; }
                public Guid CausationId { get; init; }
            }
        }
    }
}
