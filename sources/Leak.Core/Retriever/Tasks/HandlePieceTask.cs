using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Messages;
using Leak.Core.Retriever.Components;

namespace Leak.Core.Retriever.Tasks
{
    public class HandlePieceTask : LeakTask<RetrieverContext>
    {
        private readonly PeerHash peer;
        private readonly Piece piece;

        public HandlePieceTask(PeerHash peer, Piece piece)
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