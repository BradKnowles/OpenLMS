using System;

using Akka.Actor;

namespace OpenLMS.Inventory.BookUpdater.Actors
{
    public class RealDownloadActorProducer : IDownloadActorProducer
    {
        public IActorRef Create(IActorRefFactory actorRefFactory, String name) =>
            DownloadUrlActor.Create(actorRefFactory, name);

        public IActorRef Create(IActorRefFactory actorRefFactory) => Create(actorRefFactory, null);
    }

    public interface IDownloadActorProducer
    {
        IActorRef Create(IActorRefFactory actorRefFactory, String name);
        IActorRef Create(IActorRefFactory actorRefFactory);
    }
}
