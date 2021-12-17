using System;

using Akka.Actor;

namespace OpenLMS.Inventory.BookUpdater.AkkaHelpers
{
    internal static class Props
    {
        public static Akka.Actor.Props CreateUsingPrivateConstructor<TActor>(params Object[] args) where TActor : ActorBase
        {
            var producer = new InternalPrivateActivatorProducer(typeof(TActor), args);
            return Akka.Actor.Props.CreateBy(producer, args);
        }
    }
}
