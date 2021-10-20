using System;

using Akka.Actor;

using OpenLMS.Inventory.BookUpdater.Actors;

namespace OpenLMS.Inventory.BookUpdater.Coordinators
{
    internal class DownloadCoordinator : ReceiveActor
    {
        public DownloadCoordinator() =>
            Receive<Messages.DownloadUrl>(message =>
            {
                IActorRef downloadUrlActor = DownloadUrlActor.Create();
                downloadUrlActor.Forward(message);
            });

        // Receive<Messages.DownloadedContents>(message =>
        // {
        //
        //
        //
        //
        //     //Console.WriteLine(feed.Description);
        //
        // });
        public static IActorRef Create(ActorSystem actorSystem, String name = null) =>
            actorSystem.ActorOf(Props.Create<DownloadCoordinator>(),
                String.IsNullOrWhiteSpace(name) ? "downloadCoordinator" : name);

        internal static class Messages
        {
            internal record DownloadUrl(Uri Url);

            // internal record DownloadedContents(Byte[] Contents);
        }
    }
}
