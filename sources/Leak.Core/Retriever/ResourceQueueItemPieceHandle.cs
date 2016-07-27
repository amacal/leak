using Leak.Core.Common;
using Leak.Core.Messages;

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
            if (context.Storage.IsComplete(piece.Index) == false)
            {
                int blockIndex = piece.Offset / context.Repository.Properties.BlockSize;
                ResourceBlock block = new ResourceBlock(piece.Index, piece.Offset, piece.Size);

                context.Repository.SetPiece(piece.Index, blockIndex, piece.Data);
                bool completed = context.Storage.Complete(peer, block);

                if (completed)
                {
                    if (context.Repository.Verify(piece.Index) == false)
                    {
                        context.Storage.Invalidate(piece.Index);
                    }
                    else
                    {
                        context.Callback.OnPieceVerified(new ResourcePiece(piece.Index));
                    }
                }

                if (context.Storage.IsComplete())
                {
                    context.Callback.OnCompleted();
                }
            }
        }
    }
}