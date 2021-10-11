using System;

using Akka.Actor;

using OpenLMS.Inventory.BookUpdater.AkkaHelpers;

namespace OpenLMS.Inventory.BookUpdater.Actors
{
    internal class DownloadCoordinator : ReceiveActor
    {
        public DownloadCoordinator()
        {
            Receive<Messages.DownloadUrl>(message =>
            {
                IActorRef downloadUrlActor = DownloadUrlActor.Create(Context.System);
                downloadUrlActor.Tell(message);
                // Context.ActorSelection(ActorPaths.DownloadUrlActor.Path).Tell(message);
            });

            Receive<Messages.DownloadedContents>(message =>
            {




                //Console.WriteLine(feed.Description);

            });
        }

        internal class Messages
        {
            internal record DownloadUrl(Uri Url);

            internal record DownloadedContents(Byte[] Contents);
        }

        public static IActorRef Create(ActorSystem actorSystem) => actorSystem.ActorOf(Props.Create<DownloadCoordinator>(), ActorPaths.DownloadCoordinator.Name);
    }
}
