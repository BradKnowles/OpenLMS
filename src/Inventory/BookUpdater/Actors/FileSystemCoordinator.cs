// using System;
//
// using Akka.Actor;
//
// using OpenLMS.Inventory.BookUpdater.AkkaHelpers;
//
// using Zio.FileSystems;
//
// namespace OpenLMS.Inventory.BookUpdater.Actors
// {
//     internal class FileSystemCoordinator : ReceiveActor
//     {
//         public FileSystemCoordinator()
//         {
//             Receive<Messages.SaveFile>(message =>
//             {
//                 IActorRef saveFileActor = SaveFileActor.Create(Context.System, new PhysicalFileSystem());
//                 saveFileActor.Tell(message);
//             });
//         }
//
//         internal class Messages
//         {
//             internal record SaveFile(String Filename, Byte[] Contents);
//         }
//
//         public static IActorRef Create(ActorSystem actorSystem) => actorSystem.ActorOf(Props.Create<FileSystemCoordinator>(),
//             ActorPaths.FileSystemCoordinator.Name);
//     }
// }


