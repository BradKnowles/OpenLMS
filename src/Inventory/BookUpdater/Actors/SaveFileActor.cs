using System;
using System.Linq;

using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;

using OpenLMS.Inventory.BookUpdater.Coordinators;

using Zio;

namespace OpenLMS.Inventory.BookUpdater.Actors
{
    internal class SaveFileActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log;

        public SaveFileActor(IFileSystem fileSystem)
        {
            _log = Context.GetLogger<SerilogLoggingAdapter>()
                .ForContext("ActorName", $"{Self.Path.Name}#{Self.Path.Uid}");

            Receive<FileSystemSupervisor.Messages.SaveFile>(message =>
            {
                UPath currentDirectory = fileSystem.ConvertPathFromInternal(Environment.CurrentDirectory);
                UPath directory = UPath.Combine(currentDirectory, "savedFiles");
                fileSystem.CreateDirectory(directory);
                fileSystem.WriteAllBytes(UPath.Combine(directory, message.Filename), message.Contents.ToArray());

                Context.Stop(Self);
            });
        }

        protected override void PreStart() => _log.Info("Actor started");
        protected override void PostStop() => _log.Info("Actor stopped");

        public static IActorRef Create(IActorRefFactory actorRefFactor, IFileSystem fileSystem, String name = null) =>
            actorRefFactor.ActorOf(Props.Create<SaveFileActor>(fileSystem),
                String.IsNullOrWhiteSpace(name) ? "saveFile" : name);
    }
}
