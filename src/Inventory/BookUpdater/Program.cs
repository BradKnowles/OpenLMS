using System;
using System.Threading.Tasks;

using Akka.Actor;

using OpenLMS.Inventory.BookUpdater.Actors;

[assembly: CLSCompliant(true)]
namespace OpenLMS.Inventory.BookUpdater
{
    internal class Program
    {
        private static async Task Main(String[] _)
        {
            using (var actorSystem = ActorSystem.Create("BookUpdaterSystem"))
            {
                // Console.CancelKeyPress += (sender, eventArgs) =>
                // {
                //     // ReSharper disable once AccessToDisposedClosure
                //     actorSystem.Terminate().GetAwaiter().GetResult();
                //     eventArgs.Cancel = true;
                // };

                IActorRef feedCoordinator = FeedCoordinator.Create(actorSystem);
                IActorRef unused = DownloadCoordinator.Create(actorSystem);

                feedCoordinator.Tell(
                    new FeedCoordinator.Messages.DownloadFeed(
                        new Uri("https://www.gutenberg.org/cache/epub/feeds/today.rss")));

                await actorSystem.WhenTerminated.ConfigureAwait(false);
            }
        }
    }
}
