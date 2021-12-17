using System;
using System.Collections.Immutable;

using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;

using OpenLMS.Inventory.BookUpdater.Actors;
//using OpenLMS.Inventory.BookUpdater.AkkaHelpers;

using Zio;

using Props = OpenLMS.Inventory.BookUpdater.AkkaHelpers.Props;

namespace OpenLMS.Inventory.BookUpdater.Coordinators
{
    public class DownloadCoordinator : ReceiveActor
    {
        private readonly ILoggingAdapter _log;

        private DownloadCoordinator() =>
            _log = Context.GetLogger<SerilogLoggingAdapter>()
                .ForContext("ActorName", $"{Self.Path.Name}#{Self.Path.Uid}");

        internal DownloadCoordinator(IActorRef fileSystemCoordinator, IActorRef downloadUrlActor) : this()
        {
            if (fileSystemCoordinator is null) throw new ArgumentNullException(nameof(fileSystemCoordinator));

            Receive<Messages.DownloadUrl>(message =>
            {
                _log.Debug("Received {Message}", message);

                downloadUrlActor ??= DownloadUrlActor.Create(Context);
                downloadUrlActor.Tell(message);
            });

            Receive<Messages.DownloadComplete>(message =>
            {
                _log.Debug("Received {Message}", message);

                if (message.SaveToFile)
                {
                    fileSystemCoordinator.Tell(new FileSystemCoordinator.Messages.SaveFile(message.DownloadPath,
                        message.Contents, message.CorrelationId, message.MessageId));
                }
            });
        }

        public static IActorRef Create(IActorRefFactory actorRefFactory, IActorRef fileSystemSupervisor,
            String name = null) => Create(actorRefFactory, fileSystemSupervisor, null, name);

#if TEST
        public static IActorRef Create(IActorRefFactory actorRefFactory, IActorRef fileSystemSupervisor,
#else
        private static IActorRef Create(IActorRefFactory actorRefFactory, IActorRef fileSystemSupervisor,
#endif
            IActorRef downloadActor, String name = null)
        {
            if (actorRefFactory == null) throw new ArgumentNullException(nameof(actorRefFactory));
            if (fileSystemSupervisor == null) throw new ArgumentNullException(nameof(fileSystemSupervisor));

            return actorRefFactory.ActorOf(Props.CreateUsingPrivateConstructor<DownloadCoordinator>(fileSystemSupervisor, downloadActor),
                String.IsNullOrWhiteSpace(name) ? "downloadCoordinator" : name);
        }

        protected override void PreStart() => _log.Info("Actor started");
        protected override void PostStop() => _log.Info("Actor stopped");

        public static class Messages
        {
            public sealed record DownloadUrl
            {
                public DownloadUrl(Uri url, UPath downloadPath, Guid correlationId, Guid causationId)
                {
                    Url = url;
                    DownloadPath = downloadPath;
                    CorrelationId = correlationId;
                    CausationId = causationId;
                }

                public Uri Url { get; init; }
                public UPath DownloadPath { get; init; }
                public Boolean SaveToFile => String.IsNullOrWhiteSpace(DownloadPath.ToString()) == false;
                public Guid MessageId { get; init; } = Guid.NewGuid();
                public Guid CorrelationId { get; init; }
                public Guid CausationId { get; init; }
            }

            public sealed record DownloadComplete
            {
                public DownloadComplete(ImmutableArray<Byte> contents, UPath downloadPath, Guid correlationId,
                    Guid causationId)
                {
                    Contents = contents;
                    DownloadPath = downloadPath;
                    CorrelationId = correlationId;
                    CausationId = causationId;
                }

                public ImmutableArray<Byte> Contents { get; init; }
                public UPath DownloadPath { get; init; }
                public Boolean SaveToFile => String.IsNullOrWhiteSpace(DownloadPath.ToString()) == false;
                public Guid MessageId { get; init; } = Guid.NewGuid();
                public Guid CorrelationId { get; init; }
                public Guid CausationId { get; init; }
            }
        }
    }
}
