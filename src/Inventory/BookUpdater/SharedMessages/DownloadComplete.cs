using System;
using System.Collections.Immutable;

namespace OpenLMS.Inventory.BookUpdater.SharedMessages
{
    internal sealed record DownloadComplete
    {
        public DownloadComplete(ImmutableArray<Byte> contents, Guid correlationId, Guid causationId)
        {
            Contents = contents;
            CorrelationId = correlationId;
            CausationId = causationId;
        }

        public ImmutableArray<Byte> Contents { get; init; }
        public Guid MessageId { get; init; } = Guid.NewGuid();
        public Guid CorrelationId { get; init; }
        public Guid CausationId { get; init; }
    }
}
