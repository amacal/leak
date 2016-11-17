using System;

namespace Leak.Core.Glue.Extensions.Metadata
{
    public class MetadataHooks
    {
        public Action<MetadataRequest> OnMetadataRequestReceived;
    }
}