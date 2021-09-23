using System;

using Akka.Actor;

using OpenLMS.Inventory.BookUpdater.DownloadFeed.Actors;

[assembly: CLSCompliant(true)]
namespace OpenLMS.Inventory.BookUpdater
{
    internal class Program
    {
        private static void Main(String[] _)
        {
            using (var actorSystem = ActorSystem.Create(ActorData.SystemName))
            {
                IActorRef feedDownloader = actorSystem.ActorOf(Props.Create<DownloadFeedActor>(), ActorNames.DownloadFeedActor.Name);
                feedDownloader.Tell(new DownloadFeed.Messages.DownloadFeed(new Uri("https://www.gutenberg.org/cache/epub/feeds/today.rss")));

                actorSystem.WhenTerminated.Wait();
            }
        }
    }
}
