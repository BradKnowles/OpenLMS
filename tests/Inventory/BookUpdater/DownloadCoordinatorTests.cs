using System;
using System.Collections.Immutable;
using System.Text;

using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit;

using OpenLMS.Inventory.BookUpdater.Coordinators;

using Shouldly;

using Xunit;

using Zio;

namespace OpenLMS.Inventory.BookUpdater.Tests
{
    public class DownloadCoordinatorTests : TestKit
    {
        [Fact]
        public void DownloadCoordinator_DownloadUrlMsg_SendToDownloadActor()
        {
            TestProbe fileSystemCoordinator = CreateTestProbe();
            TestProbe downloadUrlActor = CreateTestProbe();
            var message = new DownloadCoordinator.Messages.DownloadUrl(new Uri("https://www.example.com"),
                new UPath("path/to/save/file.ext"),
                Guid.NewGuid(), Guid.NewGuid());

            IActorRef sut = DownloadCoordinator.Create(Sys, fileSystemCoordinator, downloadUrlActor);
            sut.Tell(message);

            DownloadCoordinator.Messages.DownloadUrl result = downloadUrlActor.ExpectMsg<DownloadCoordinator.Messages.DownloadUrl>();
            result.Url.ShouldBe(message.Url);
            result.SaveToFile.ShouldBe(true);
            result.DownloadPath.ShouldBe(message.DownloadPath);
            result.CorrelationId.ShouldBe(message.CorrelationId);
            result.CausationId.ShouldBe(message.CausationId);
        }

        [Fact]
        public void DownloadCoordinator_DownloadCompleteMsg_SendSaveFileMsg()
        {
            TestProbe fileSystemCoordinator = CreateTestProbe();
            TestProbe downloadUrlActor = CreateTestProbe();
            var downloadContents = Encoding.UTF8.GetBytes("Downloaded data.").ToImmutableArray();

            var message = new DownloadCoordinator.Messages.DownloadComplete(downloadContents, new UPath("path/to/save/file.ext"),
                Guid.NewGuid(), Guid.NewGuid());

            IActorRef sut = DownloadCoordinator.Create(Sys, fileSystemCoordinator, downloadUrlActor);
            sut.Tell(message);

            FileSystemCoordinator.Messages.SaveFile result = fileSystemCoordinator.ExpectMsg<FileSystemCoordinator.Messages.SaveFile>();
            result.Contents.ShouldBe(downloadContents);
            result.Filename.ShouldBe(message.DownloadPath);
            result.CorrelationId.ShouldBe(message.CorrelationId);
            result.CausationId.ShouldBe(message.MessageId);
        }

        [Fact]
        public void DownloadCoordinator_CreateWithNull_ThrowsException()
            => Should.Throw<ArgumentNullException>(() => DownloadCoordinator.Create(Sys, null));
    }
}
