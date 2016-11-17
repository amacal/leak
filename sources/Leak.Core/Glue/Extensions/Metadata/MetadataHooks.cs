using System;

namespace Leak.Core.Glue.Extensions.Metadata
{
    public class MetadataHooks
    {
        public Action<MetadataMeasured> OnMetadataMeasured;

        public Action<MetadataRequested> OnMetadataRequested;

        public Action<MetadataRejected> OnMetadataRejected;

        public Action<MetadataReceived> OnMetadataReceived;
    }
}