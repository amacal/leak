using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Omnibus;
using Leak.Core.Repository;

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
                RepositoryBlockData block = new RepositoryBlockData(piece.Index, piece.Offset, piece.Data);

                context.Repository.Write(block);
                bool completed = context.Omnibus.Complete(new OmnibusBlock(piece.Index, piece.Offset, piece.Size));

                if (completed)
                {
                    context.Repository.Verify(new RepositoryPiece(piece.Index));
                }

                if (context.Omnibus.IsComplete())
                {
                    context.Callback.OnCompleted(context.Metainfo.Hash);
                }

                context.Collector.Increase(peer, 2);
            }
        }
    }
}