using System;

using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;

namespace OpenLMS.Inventory.BookUpdater.Coordinators
{
    internal class BookUpdaterSupervisor : UntypedActor
    {
        private readonly ILoggingAdapter _log;

        public BookUpdaterSupervisor()
        {
            _log = Context.GetLogger<SerilogLoggingAdapter>()
                .ForContext("ActorName", $"{Self.Path.Name}#{Self.Path.Uid}");

            IActorRef downloadCoordinator = DownloadCoordinator.Create(Context);
            IActorRef feedCoordinator = FeedCoordinator.Create(Context, downloadCoordinator);

            feedCoordinator.Tell(
                new FeedCoordinator.Messages.DownloadFeed(
                    new Uri("https://www.gutenberg.org/cache/epub/feeds/today.rss")));
        }

        protected override void PreStart() => _log.Info("Actor started");
        protected override void PostStop() => _log.Info("Actor stopped");

        protected override void OnReceive(Object message) { }

        public static IActorRef Create(ActorSystem actorSystem, String name = null) =>
            actorSystem.ActorOf(Props.Create<BookUpdaterSupervisor>(),
                String.IsNullOrWhiteSpace(name) ? "bookUpdaterSupervisor" : name);
    }
}
