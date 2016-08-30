using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Retriever
{
    public class RetrieverTaskPiece : RetrieverTask
    {
        private readonly PeerHash peer;
        private readonly Piece piece;

        public RetrieverTaskPiece(PeerHash peer, Piece piece)
        {
            this.peer = peer;
            this.piece = piece;
        }

        public void Execute(RetrieverContext context)
        {
            if (context.Omnibus.IsComplete(piece.Index) == false)
            {
                context.Repository.Write(piece.Index, piece.Offset, piece.Data);
                context.Collector.Increase(peer, 2);
            }
        }
    }
}