using System;
using System.Threading.Tasks;

using Akka.Actor;

using OpenLMS.Inventory.BookUpdater.Coordinators;

using Serilog;
using Serilog.Core;

[assembly: CLSCompliant(true)]

namespace OpenLMS.Inventory.BookUpdater
{
    internal class Program
    {
        private static async Task Main()
        {
            ConfigureLogging();

            const String akkaConfigWithLogging =
                "akka {actor.debug.unhandled = on, loglevel = DEBUG, loggers = [\"Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog\"]}";
            using (var actorSystem = ActorSystem.Create("BookUpdaterSystem", akkaConfigWithLogging))
            {
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    Task shutdownTask = CoordinatedShutdown.Get(actorSystem)
                        .Run(CoordinatedShutdown.ClrExitReason.Instance);
                    shutdownTask.GetAwaiter().GetResult();
                    eventArgs.Cancel = true;
                };

                _ = BookUpdaterSupervisor.Create(actorSystem);

                await actorSystem.WhenTerminated.ConfigureAwait(false);
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureLogging()
        {
            Logger logger = new LoggerConfiguration()
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:HH:mm:ss} {Level:u3}] [{ActorName}] {Message:lj}{NewLine}{Exception}")
                .MinimumLevel.Debug()
                .CreateLogger();

            Log.Logger = logger;
        }
    }
}
