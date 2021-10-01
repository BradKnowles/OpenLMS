using System;
using System.Threading.Tasks;

using Akka.Actor;

using OpenLMS.Inventory.BookUpdater.AkkaHelpers;

namespace OpenLMS.Inventory.BookUpdater.Actors
{
    internal class FeedCoordinator : ReceiveActor
    {
        public FeedCoordinator()
        {
            ReceiveAsync<Messages.DownloadFeed>(async message =>
            {
                Console.WriteLine(message.FeedUrl);
                await Task.CompletedTask.ConfigureAwait(false);

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
