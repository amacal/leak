using Leak.Core.Cando.Metadata;
using Leak.Core.Common;

namespace Leak.Core.Collector.Callbacks
{
    public class PeerCollectorToMetadata : MetadataCallbackBase
    {
        private readonly PeerCollectorContext context;

        public PeerCollectorToMetadata(PeerCollectorContext context)
        {
            this.context = context;
        }

        public override void OnData(PeerSession session, MetadataData data)
        {
            context.Callback.OnMetadataReceived(session, data);
        }
    }
}