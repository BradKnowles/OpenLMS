using System;

using Akka.Actor;

namespace OpenLMS.Inventory.BookUpdater
{
    internal class DownloadFeedActor : ReceiveActor
    {
        public DownloadFeedActor()
        {
            Receive<String>(s => s.StartsWith("AkkaDotNet", StringComparison.InvariantCulture), s =>
             {
                 // handle string
             });


            Receive<String>(s => s.StartsWith("AkkaDotNet", StringComparison.InvariantCulture), s =>
             {
                 // handle string
             });
        }
    }
}
