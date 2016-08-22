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

        public override void OnSize(PeerSession session, MetadataSize size)
        {
            context.Callback.OnMetadataSize(session, size);
        }

        public override void OnData(PeerSession session, MetadataData data)
        {
            context.Callback.OnMetadataReceived(session, data);
        }
    }
}