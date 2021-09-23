using System;

using Akka.Actor;

namespace OpenLMS.Inventory.BookUpdater.DownloadFeed.Actors
{
    internal class DownloadFeedActor : ReceiveActor
    {
        public DownloadFeedActor()
        {
            Receive<Messages.DownloadFeed>(message => { Console.WriteLine(message.FeedUrl); });


            Receive<String>(s => s.StartsWith("AkkaDotNet", StringComparison.InvariantCulture), s =>
            {
                // handle string
            });
        }
    }
}
