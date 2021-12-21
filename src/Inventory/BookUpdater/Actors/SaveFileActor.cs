using System;
using System.IO;
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

            Receive<FileSystemCoordinator.Messages.SaveFile>(message =>
            {
                _log.Debug("Received message {Message}", message);

                UPath currentDirectory = fileSystem.ConvertPathFromInternal(Environment.CurrentDirectory);
                var fullPath = UPath.Combine(currentDirectory, message.Filename);

                fileSystem.CreateDirectory(fullPath.GetDirectory());
                fileSystem.WriteAllBytes(fullPath, message.Contents.ToArray());

                Context.Stop(Self);
            });
        }

        protected override void PreStart() => _log.Info("Actor started");
        protected override void PostStop() => _log.Info("Actor stopped");

        public static IActorRef Create(IActorRefFactory actorRefFactor, IFileSystem fileSystem, String name = null)
        {
            String actorName = $"{(String.IsNullOrWhiteSpace(name) ? "downloadUrl" : name)}-{Path.GetRandomFileName().Substring(0, 5)}";
            return actorRefFactor.ActorOf(Props.Create<SaveFileActor>(fileSystem), actorName);
        }
    }
}
