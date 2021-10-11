using System;

using Akka.Actor;

using OpenLMS.Inventory.BookUpdater.AkkaHelpers;

using Zio;

namespace OpenLMS.Inventory.BookUpdater.Actors
{
    internal class SaveFileActor : ReceiveActor
    {
        private readonly IFileSystem _fileSystem;

        public SaveFileActor(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;

            Receive<FileSystemCoordinator.Messages.SaveFile>(message =>
            {
                UPath currentDirectory = _fileSystem.ConvertPathFromInternal(Environment.CurrentDirectory);
                var directory = UPath.Combine(currentDirectory, "savedFiles");
                _fileSystem.CreateDirectory(directory);
                _fileSystem.WriteAllBytes(UPath.Combine(directory, message.Filename), message.Contents);
            });
        }


        public static IActorRef Create(ActorSystem actorSystem, IFileSystem fileSystem) => actorSystem.ActorOf(Props.Create<SaveFileActor>(fileSystem), ActorPaths.SaveFileActor.Name);
    }
}
