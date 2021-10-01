using System;

namespace OpenLMS.Inventory.BookUpdater.AkkaHelpers
{
    /// <summary>
    /// Meta-data class. Nested/child actors can build path
    /// based on their parent(s) / position in hierarchy.
    /// </summary>
    internal class ActorMetaData
    {
        public ActorMetaData(String name, ActorMetaData parent = null)
        {
            Name = name;
            Parent = parent;
            // if no parent, we assume a top-level actor
            String parentPath = parent != null ? parent.Path : "/user";
            Path = $"{parentPath}/{Name}";
        }

        public String Name { get; }
        public ActorMetaData Parent { get; }
        public String Path { get; }
    }
}
