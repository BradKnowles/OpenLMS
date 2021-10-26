using System;
using System.Collections.Immutable;

using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;

using Flurl.Http;

using OpenLMS.Inventory.BookUpdater.Coordinators;
using OpenLMS.Inventory.BookUpdater.SharedMessages;

namespace OpenLMS.Inventory.BookUpdater.Actors
{
    internal class DownloadUrlActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log;

        public DownloadUrlActor()
        {
            _log = Context.GetLogger<SerilogLoggingAdapter>()
                .ForContext("ActorName", $"{Self.Path.Name}#{Self.Path.Uid}");

            ReceiveAsync<DownloadCoordinator.Messages.DownloadUrl>(async message =>
            {
                _log.Debug("Received message {Message}", message);

                IFlurlResponse response = await message.Url.GetAsync().ConfigureAwait(true);
                _ = response.ResponseMessage.EnsureSuccessStatusCode();

                Byte[] contents = await response.GetBytesAsync().ConfigureAwait(true);
                Sender.Tell(new DownloadComplete(contents.ToImmutableArray(), message.CorrelationId,
                    message.MessageId));

                Context.Stop(Self);
            });
        }

        protected override void PreStart() => _log.Info("Actor started");
        protected override void PostStop() => _log.Info("Actor stopped");

        public static IActorRef Create(IActorRefFactory actorRefFactory, String name = null) =>
            actorRefFactory.ActorOf(Props.Create<DownloadUrlActor>(),
                String.IsNullOrWhiteSpace(name) ? "downloadUrl" : name);

        internal static class Messages
        {
            // internal record DownloadedContents(Byte[] contents);
        }
    }
}
