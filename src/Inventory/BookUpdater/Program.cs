using System;

using Akka.Actor;

[assembly: CLSCompliant(true)]
namespace OpenLMS.Inventory.BookUpdater
{
    internal class Program
    {
#pragma warning disable IDE0060,CA1801 // Remove unused parameter
        private static void Main(String[] args)
#pragma warning restore IDE0060,CA1801 // Remove unused parameter
        {
            var actorSystem = ActorSystem.Create("BookUpdaterSystem");


            actorSystem.Dispose();
            //actorSystem
        }
    }
}
