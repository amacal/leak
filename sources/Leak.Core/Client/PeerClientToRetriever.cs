using Leak.Core.Metadata;
using Leak.Core.Retriever;

namespace Leak.Core.Client
{
    public class PeerClientToRetriever : ResourceRetrieverCallbackBase
    {
        private readonly Metainfo metainfo;
        private readonly PeerClientConfiguration configuration;
        private readonly PeerClientCallback callback;

        public PeerClientToRetriever(Metainfo metainfo, PeerClientConfiguration configuration)
        {
            this.metainfo = metainfo;
            this.configuration = configuration;
            this.callback = configuration.Callback;
        }

        public override void OnCompleted()
        {
            callback.OnCompleted(metainfo);
        }

        public override void OnPieceVerified(ResourcePiece piece)
        {
            callback.OnPieceVerified(metainfo, new PeerClientPieceVerification(piece));
        }
    }
}