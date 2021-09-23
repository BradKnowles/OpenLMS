using System;

namespace OpenLMS.Inventory.BookUpdater
{
    internal record ActorData
    {
        public ActorData(String name) : this(name, $"akka://{SystemName}/user") { }

        public ActorData(String name, String parent)
        {
            if (String.IsNullOrWhiteSpace(parent))
                throw new ArgumentNullException(nameof(parent));

            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Path = $"{parent}/{name}";
        }

        public String Name { get; init; }
        public String Path { get; init; }

        public static String SystemName => "BookUpdaterSystem";
    }
}
