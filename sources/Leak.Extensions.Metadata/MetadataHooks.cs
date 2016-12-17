using System;

namespace Leak.Extensions.Metadata
{
    public class MetadataHooks
    {
        public Action<MetadataMeasured> OnMetadataMeasured;

        public Action<MetadataRequested> OnMetadataRequestSent;
        public Action<MetadataRequested> OnMetadataRequestReceived;

        public Action<MetadataRejected> OnMetadataRejectSent;
        public Action<MetadataRejected> OnMetadataRejectReceived;

        public Action<MetadataReceived> OnMetadataPieceSent;
        public Action<MetadataReceived> OnMetadataPieceReceived;
    }
}