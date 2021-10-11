using System;

using Akka.Actor;

using OpenLMS.Inventory.BookUpdater.AkkaHelpers;

namespace OpenLMS.Inventory.BookUpdater.Actors
{
    internal class FeedCoordinator : ReceiveActor
    {
        public FeedCoordinator()
        {
            Receive<Messages.DownloadFeed>(message =>
            {
                var downloadMessage = new DownloadCoordinator.Messages.DownloadUrl(message.FeedUrl);
                ActorSelection downloadCoordinator = Context.ActorSelection(ActorPaths.DownloadCoordinator.Path);
                downloadCoordinator.Tell(downloadMessage);


                // FeedReader.
            });
        }

        public static IActorRef Create(ActorSystem actorSystem) => actorSystem.ActorOf(Props.Create<FeedCoordinator>(), ActorPaths.FeedCoordinatorActor.Name);

        public class Messages
        {
            internal record DownloadFeed(Uri FeedUrl);
        }
    }
}
