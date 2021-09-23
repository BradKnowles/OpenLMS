using System;

namespace OpenLMS.Inventory.BookUpdater
{
    internal static class ActorNames
    {
        /// <summary>
        /// Responsible for serializing writes to the <see cref="Console"/>
        /// </summary>
        //public static readonly ActorData ConsoleWriterActor = new("consoleWriter"); // /user/consoleWriter

        public static readonly ActorData DownloadFeedActor = new("downloadFeed");

        //public static readonly ActorData FeedValidatorActor = new ActorData("feedValidator", "akka://MyFirstActorSystem/user"); // /user/feedValidator

    }
}
