using System;
using System.Reflection;

using Akka.Actor;

namespace OpenLMS.Inventory.BookUpdater.AkkaOverrides
{
    internal class PrivateActivatorProducer : IIndirectActorProducer
    {
        private readonly Object[] _args;

        public PrivateActivatorProducer(Type actorType, params Object[] args)
        {
            ActorType = actorType;
            _args = args;
        }
        public Type ActorType { get; }

        public ActorBase Produce()
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            return (ActorBase)Activator.CreateInstance(ActorType, flags, null, _args, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "This is a direct copy of the method from Akka.NET.")]
        public void Release(ActorBase actor) => actor = null;
    }
}

