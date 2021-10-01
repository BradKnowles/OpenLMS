using Akka.Actor;

using OpenLMS.Inventory.BookUpdater.AkkaHelpers;

namespace OpenLMS.Inventory.BookUpdater.Actors
{
    internal class DownloadCoordinator : ReceiveActor
    {
        public DownloadCoordinator()
        {

        }

        public static IActorRef Create(ActorSystem actorSystem) => actorSystem.ActorOf(Props.Create<DownloadCoordinator>(), ActorPaths.DownloadCoordinator.Name);
    }
}
