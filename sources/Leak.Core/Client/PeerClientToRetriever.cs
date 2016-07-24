using Leak.Core.Common;
using Leak.Core.Retriever;

namespace Leak.Core.Client
{
    public class PeerClientToRetriever : ResourceRetrieverCallbackBase
    {
        private readonly FileHash hash;
        private readonly PeerClientConfiguration configuration;
        private readonly PeerClientStorage storage;
        private readonly PeerClientCallback callback;

        public PeerClientToRetriever(FileHash hash, PeerClientConfiguration configuration, PeerClientStorage storage)
        {
            this.hash = hash;
            this.configuration = configuration;
            this.storage = storage;
            this.callback = configuration.Callback;
        }

        public override void OnCompleted()
        {
            callback.OnCompleted(hash);
        }

        public override void OnPieceVerified(ResourcePiece piece)
        {
            callback.OnPieceVerified(hash, new PeerClientPieceVerification(piece));
        }

        public override void OnMetadataCompleted()
        {
            storage.WithMetainfo(hash);
        }
    }
}