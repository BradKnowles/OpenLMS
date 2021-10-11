using System;

using Akka.Actor;

using Flurl.Http;

using OpenLMS.Inventory.BookUpdater.AkkaHelpers;

namespace OpenLMS.Inventory.BookUpdater.Actors
{
    internal class DownloadUrlActor : ReceiveActor
    {
        private readonly IActorRef _sender;

        public DownloadUrlActor()
        {
            _sender = Sender;
            ReceiveAsync<DownloadCoordinator.Messages.DownloadUrl>(async message =>
            {
                IFlurlResponse response = await message.Url.GetAsync().ConfigureAwait(true);
                var unused = response.ResponseMessage.EnsureSuccessStatusCode();
                Byte[] contents = await response.GetBytesAsync().ConfigureAwait(true);

                IActorRef fileSystemCoordinator = FileSystemCoordinator.Create(Context.System);
                fileSystemCoordinator.Tell(new FileSystemCoordinator.Messages.SaveFile("today.rss", contents));
            });
        }

        internal class Messages { }

        public static IActorRef Create(ActorSystem actorSystem) =>
            actorSystem.ActorOf(Props.Create<DownloadUrlActor>(), ActorPaths.DownloadUrlActor.Name);
    }
}
