using Leak.Core.Common;
using Leak.Core.Extensions.Metadata;

namespace Leak.Core.Client
{
    public class PeerClientToMetadata : MetadataCallbackBase
    {
        private readonly PeerClientExtensionContext context;

        public PeerClientToMetadata(PeerClientExtensionContext context)
        {
            this.context = context;
        }

        public override void OnData(PeerHash peer, MetadataData data)
        {
            FileHash hash = context.GetHash(peer);

            context.GetRetriever(peer).AddMetadata(peer, data);
            context.GetCallback(peer).OnMetadataReceived(hash, peer, data);
        }
    }
}