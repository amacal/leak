using Leak.Core.Events;
using System;

namespace Leak.Core.Metaget
{
    public class MetagetHooks
    {
        public Action<MetafileMeasured> OnMetadataMeasured;

        public Action<MetadataPieceRequested> OnMetadataPieceRequested;

        public Action<MetadataPieceReceived> OnMetadataPieceReceived;

        public Action<MetadataDiscovered> OnMetadataDiscovered;
    }
}