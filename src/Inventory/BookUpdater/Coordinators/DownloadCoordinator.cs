using System;

using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;

using OpenLMS.Inventory.BookUpdater.Actors;

namespace OpenLMS.Inventory.BookUpdater.Coordinators
{
    internal class DownloadCoordinator : ReceiveActor
    {
        private readonly ILoggingAdapter _log;

        public DownloadCoordinator()
        {
            _log = Context.GetLogger<SerilogLoggingAdapter>()
                .ForContext("ActorName", $"{Self.Path.Name}#{Self.Path.Uid}");

            Receive<Messages.DownloadUrl>(message =>
            {
                IActorRef downloadUrlActor = DownloadUrlActor.Create(Context);
                downloadUrlActor.Forward(message);
            });
        }

        public static IActorRef Create(IActorRefFactory actorRefFactory, String name = null) =>
            actorRefFactory.ActorOf(Props.Create<DownloadCoordinator>(),
                String.IsNullOrWhiteSpace(name) ? "downloadCoordinator" : name);

        protected override void PreStart() => _log.Info("Actor started");
        protected override void PostStop() => _log.Info("Actor stopped");

        internal static class Messages
        {
            internal sealed record DownloadUrl
            {
                public DownloadUrl(Uri url, Guid correlationId, Guid causationId)
                {
                    Url = url;
                    CorrelationId = correlationId;
                    CausationId = causationId;
                }

                public Uri Url { get; init; }
                public Guid MessageId { get; init; } = Guid.NewGuid();
                public Guid CorrelationId { get; init; }
                public Guid CausationId { get; init; }
            }
        }
    }
}
