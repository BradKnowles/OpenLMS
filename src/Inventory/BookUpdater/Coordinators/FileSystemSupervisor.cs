using System;
using System.Collections.Immutable;

using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;

using OpenLMS.Inventory.BookUpdater.Actors;

using Zio.FileSystems;

namespace OpenLMS.Inventory.BookUpdater.Coordinators
{
    internal class FileSystemSupervisor : ReceiveActor
    {
        private readonly ILoggingAdapter _log;

        public FileSystemSupervisor()
        {
            _log = Context.GetLogger<SerilogLoggingAdapter>()
                .ForContext("ActorName", $"{Self.Path.Name}#{Self.Path.Uid}");

            Receive<Messages.SaveFile>(message =>
            {
                IActorRef saveFileActor = SaveFileActor.Create(Context.System, new PhysicalFileSystem());
                saveFileActor.Tell(message);
            });
        }

        protected override void PreStart() => _log.Info("Actor started");
        protected override void PostStop() => _log.Info("Actor stopped");

        public static IActorRef Create(IActorRefFactory actorRefFactory, String name = null) =>
            actorRefFactory.ActorOf(Props.Create<FileSystemSupervisor>(),
                String.IsNullOrWhiteSpace(name) ? "fileSystemSupervisor" : name);

        internal class Messages
        {
            internal sealed record SaveFile
            {
                public SaveFile(String filename, ImmutableArray<Byte> contents, Guid correlationId, Guid causationId)
                {
                    Filename = filename;
                    Contents = contents;
                    CorrelationId = correlationId;
                    CausationId = causationId;
                }

                public String Filename { get; init; }
                public ImmutableArray<Byte> Contents { get; init; }
                public Guid MessageId { get; init; } = Guid.NewGuid();
                public Guid CorrelationId { get; init; }
                public Guid CausationId { get; init; }
            }
        }
    }
}
