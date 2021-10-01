using System;
using System.Net.Http;

using Akka.Actor;

using Flurl.Http;

using OpenLMS.Inventory.BookUpdater.AkkaHelpers;

namespace OpenLMS.Inventory.BookUpdater.DownloadUrl
{
    internal class DownloadUrlActor : ReceiveActor
    {
        public DownloadUrlActor()
        {
            ReceiveAsync<Messages.DownloadUrl>(async message =>
            {
                IFlurlResponse response = await message.Url.GetAsync().ConfigureAwait(false);
                HttpResponseMessage httpResponse = response.ResponseMessage.EnsureSuccessStatusCode();
                Sender.Tell(await response.GetBytesAsync().ConfigureAwait(false));
            });
        }

        internal class Messages
        {
            internal record DownloadUrl(Uri Url);
        }

        public static IActorRef Create(ActorSystem actorSystem) => actorSystem.ActorOf(Props.Create<DownloadUrlActor>(), ActorPaths.DownloadUrlActor.Name);
    }
}
