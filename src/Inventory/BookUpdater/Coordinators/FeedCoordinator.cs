using System;

using Akka.Actor;

using CodeHollow.FeedReader;

using OpenLMS.Inventory.BookUpdater.Actors;

namespace OpenLMS.Inventory.BookUpdater.Coordinators
{
    internal class FeedCoordinator : ReceiveActor
    {
        public FeedCoordinator(IActorRef downloadCoordinator)
        {
            Receive<Messages.DownloadFeed>(message =>
            {
                var downloadMessage = new DownloadCoordinator.Messages.DownloadUrl(message.FeedUrl);
                downloadCoordinator.Tell(downloadMessage);
            });

            Receive<Messages.DownloadedContents>(message =>
            {
                IActorRef parseFeedActor = ParseFeedActor.Create();
                parseFeedActor.Tell(new ParseFeedActor.Messages.ParseFeed(message.Contents));
            });
        }


        public static IActorRef Create(ActorSystem actorSystem, IActorRef downloadCoordinator, String name = null) =>
            actorSystem.ActorOf(Props.Create<FeedCoordinator>(downloadCoordinator),
                String.IsNullOrWhiteSpace(name) ? "feedCoordinator" : name);

        internal static class Messages
        {
            internal record DownloadFeed(Uri FeedUrl);

            internal record DownloadedContents(Byte[] Contents);

            internal record ParsedFeed(Feed Feed);
        }
    }
}
