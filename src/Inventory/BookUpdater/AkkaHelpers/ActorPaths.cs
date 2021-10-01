namespace OpenLMS.Inventory.BookUpdater.AkkaHelpers
{
    internal static class ActorPaths
    {
        public static readonly ActorMetaData FeedCoordinatorActor = new("feedCoordinator");
        public static readonly ActorMetaData DownloadCoordinator = new("downloadCoordinator");
        public static readonly ActorMetaData DownloadUrlActor = new("downloadUrl", DownloadCoordinator);
    }
}
