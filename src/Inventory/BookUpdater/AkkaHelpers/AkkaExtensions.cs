using System;

namespace OpenLMS.Inventory.BookUpdater.AkkaHelpers
{
    public static class AkkaExtensions
    {
        public static T AsInstanceOf<T>(this Object self) => (T)self;
    }
}
