using System;

using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit;

using OpenLMS.Inventory.BookUpdater.Coordinators;

using Shouldly;

using Xunit;

namespace OpenLMS.Inventory.BookUpdater.Tests
{
    public class FeedCoordinatorTests : TestKit
    {
        [Fact]
        public void FeedCoordinator_DownloadFeedMsg_SendToDownloadCoordinator()
        {
            TestProbe downloadCoordinator = CreateTestProbe();
            TestProbe fileSystemSupervisor = CreateTestProbe();
            IActorRef sut = FeedCoordinator.Create(Sys, downloadCoordinator, fileSystemSupervisor);

            var message = new FeedCoordinator.Messages.DownloadFeed(new Uri("https://example.com/feed.rss"));
            sut.Tell(message);

            DownloadCoordinator.Messages.DownloadUrl result = downloadCoordinator.ExpectMsg<DownloadCoordinator.Messages.DownloadUrl>();
            result.Url.ShouldBe(message.FeedUrl);
            result.CorrelationId.ShouldBe(message.CorrelationId);
            result.CausationId.ShouldBe(message.CausationId);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void FeedCoordinator_CreateWithNull_ThrowsException(Boolean useTestProbeForDownloadCoordinator, Boolean useTestProbleForFileSystemSupervisor)
        {
            TestProbe downloadCoordinator = useTestProbeForDownloadCoordinator ? CreateTestProbe() : null;
            TestProbe fileSystemSupervisor = useTestProbleForFileSystemSupervisor ? CreateTestProbe() : null;
            _ = Should.Throw<ArgumentNullException>(() => FeedCoordinator.Create(Sys, downloadCoordinator, fileSystemSupervisor));
        }
    }
}
