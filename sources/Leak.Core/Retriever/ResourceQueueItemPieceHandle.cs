using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Omnibus;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemPieceHandle : ResourceQueueItem
    {
        private readonly PeerHash peer;
        private readonly Piece piece;

        public ResourceQueueItemPieceHandle(PeerHash peer, Piece piece)
        {
            this.peer = peer;
            this.piece = piece;
        }

        public void Handle(ResourceQueueContext context)
        {
            if (context.Omnibus.IsComplete(piece.Index) == false)
            {
                int blockIndex = piece.Offset / context.Properties.BlockSize;
                OmnibusBlock block = new OmnibusBlock(piece.Index, piece.Offset, piece.Size);

                context.Repository.SetPiece(piece.Index, blockIndex, piece.Data);
                bool completed = context.Omnibus.Complete(block);

                if (completed)
                {
                    if (context.Repository.Verify(piece.Index) == false)
                    {
                        context.Omnibus.Invalidate(piece.Index);
                    }
                    else
                    {
                        context.Callback.OnPieceVerified(new ResourcePiece(piece.Index));
                    }
                }

                if (context.Omnibus.IsComplete())
                {
                    context.Callback.OnCompleted();
                }
            }
        }
    }
}