using System;

using Akka.Actor;

namespace OpenLMS.Inventory.BookUpdater.AkkaOverrides
{
    public static class Props
    {
        public static Akka.Actor.Props CreatePrivate<TActor>(params Object[] args) where TActor : ActorBase
        {
            var producer = new PrivateActivatorProducer(typeof(TActor), args);
            return Akka.Actor.Props.CreateBy(producer, args);
        }
    }
}
