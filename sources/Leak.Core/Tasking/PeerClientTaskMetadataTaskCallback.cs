using Leak.Core.Cando.Metadata;
using Leak.Core.Common;

namespace Leak.Core.Tasking
{
    public class PeerClientTaskMetadataTaskCallback : PeerClientTaskCallbackBase
    {
        private readonly PeerClientTaskMetadataContext context;

        public PeerClientTaskMetadataTaskCallback(PeerClientTaskMetadataContext context)
        {
            this.context = context;
        }

        public override void OnMetadataSize(PeerHash peer, MetadataSize size)
        {
            context.Metaget.OnSize(peer, size);
        }

        public override void OnMetadataData(PeerHash peer, MetadataData data)
        {
            context.Metaget.OnData(peer, data);
        }
    }
}