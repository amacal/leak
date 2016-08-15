using Leak.Core.Cando.Metadata;
using Leak.Core.Common;

namespace Leak.Core.Collector.Callbacks
{
    public class PeerCollectorMetadata : MetadataCallbackBase
    {
        private readonly PeerCollectorContext context;

        public PeerCollectorMetadata(PeerCollectorContext context)
        {
            this.context = context;
        }

        public override void OnSize(PeerHash peer, MetadataSize size)
        {
            context.Callback.OnMetadataSize(peer, size);
        }

        public override void OnData(PeerHash peer, MetadataData data)
        {
            context.Callback.OnMetadataReceived(peer, data);
        }
    }
}