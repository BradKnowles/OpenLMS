using System;
using System.Reflection;

using Akka.Actor;

namespace OpenLMS.Inventory.BookUpdater.AkkaHelpers
{
    internal class InternalPrivateActivatorProducer : IIndirectActorProducer
    {
        private readonly Object[] _args;

        public InternalPrivateActivatorProducer(Type actorType, params Object[] args)
        {
            ActorType = actorType;
            _args = args;
        }
        public Type ActorType { get; }

        public ActorBase Produce()
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            return Activator.CreateInstance(ActorType, flags, null, _args, null).AsInstanceOf<ActorBase>();
        }
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        public void Release(ActorBase actor) => actor = null;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }
}

