using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Tasking
{
    public class PeerClientTaskDownloadTaskCallback : PeerClientTaskCallbackBase
    {
        private readonly PeerClientTaskDownloadContext context;

        public PeerClientTaskDownloadTaskCallback(PeerClientTaskDownloadContext context)
        {
            this.context = context;
        }

        public override void OnPeerBitfield(PeerHash peer, Bitfield bitfield)
        {
            context.Retriever.OnBitfield(peer, bitfield);
        }

        public override void OnPeerPiece(PeerHash peer, Piece piece)
        {
            context.Retriever.OnPiece(peer, piece);
        }
    }
}