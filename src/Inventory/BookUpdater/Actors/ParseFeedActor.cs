using System;

using Akka.Actor;

using CodeHollow.FeedReader;

using OpenLMS.Inventory.BookUpdater.Coordinators;

namespace OpenLMS.Inventory.BookUpdater.Actors
{
    internal class ParseFeedActor : ReceiveActor
    {
        public ParseFeedActor() =>
            Receive<Messages.ParseFeed>(message =>
            {
                Feed feed = FeedReader.ReadFromByteArray(message.FeedContents);
                Console.WriteLine(feed.Description);
                Sender.Tell(new FeedCoordinator.Messages.ParsedFeed(feed));
            });

        public static IActorRef Create( /*ActorSystem actorSystem,*/ String name = null) =>
            Context.ActorOf(Props.Create<ParseFeedActor>(),
                String.IsNullOrWhiteSpace(name) ? "parseFeed" : name);

        internal static class Messages
        {
            internal record ParseFeed(Byte[] FeedContents);
        }
    }
}
