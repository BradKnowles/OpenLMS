using System;

using Akka.Actor;

using Flurl.Http;

using OpenLMS.Inventory.BookUpdater.Coordinators;

namespace OpenLMS.Inventory.BookUpdater.Actors
{
    internal class DownloadUrlActor : ReceiveActor
    {
        // private readonly IActorRef _sender;

        public DownloadUrlActor() =>
            // _sender = Sender;
            ReceiveAsync<DownloadCoordinator.Messages.DownloadUrl>(async message =>
            {
                IFlurlResponse response = await message.Url.GetAsync().ConfigureAwait(true);
                _ = response.ResponseMessage.EnsureSuccessStatusCode();
                Byte[] contents = await response.GetBytesAsync().ConfigureAwait(true);
                Sender.Tell(new FeedCoordinator.Messages.DownloadedContents(contents));

                // IActorRef fileSystemCoordinator = FileSystemCoordinator.Create(Context.System);
                // fileSystemCoordinator.Tell(new FileSystemCoordinator.Messages.SaveFile("today.rss", contents));
            });

        public static IActorRef Create( /*ActorSystem actorSystem, */ String name = null) =>
            Context.ActorOf(Props.Create<DownloadUrlActor>(),
                String.IsNullOrWhiteSpace(name) ? "downloadUrl" : name);

        internal static class Messages
        {
            // internal record DownloadedContents(Byte[] contents);
        }
    }
}
