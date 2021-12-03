using System;

using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit;

using OpenLMS.Inventory.BookUpdater.Actors;

namespace OpenLMS.Inventory.BookUpdater.Tests
{
    public sealed class TestDownloadActorProducer : TestKit, IDownloadActorProducer
    {
        public TestDownloadActorProducer() : base()
        {
            Probe = CreateTestProbe();
        }
#pragma warning disable CA1051
        public TestProbe Probe;
#pragma warning restore CA1051

        public IActorRef Create(IActorRefFactory actorRefFactory, String name) => Probe;

        public IActorRef Create(IActorRefFactory actorRefFactory) => Create(actorRefFactory, null);
    }
}
