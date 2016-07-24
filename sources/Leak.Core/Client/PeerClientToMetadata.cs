using Leak.Core.Common;
using Leak.Core.Extensions.Metadata;

namespace Leak.Core.Client
{
    public class PeerClientToMetadata : MetadataCallbackBase
    {
        private readonly PeerClientCallback callback;
        private readonly PeerClientStorage storage;

        public PeerClientToMetadata(PeerClientConfiguration configuration, PeerClientStorage storage)
        {
            this.callback = configuration.Callback;
            this.storage = storage;
        }

        public override void OnData(PeerHash peer, MetadataData data)
        {
            FileHash hash = storage.GetHash(peer);

            storage.GetRetriever(peer).AddMetadata(peer, data);
            callback.OnMetadataReceived(hash, peer, data);
        }
    }
}