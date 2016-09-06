using Leak.Core.Common;
using Leak.Core.Omnibus;
using Leak.Core.Repository;
using Leak.Core.Retriever.Components;

namespace Leak.Core.Retriever.Callbacks
{
    public class RetrieverToRepository : RepositoryCallbackBase
    {
        private readonly RetrieverContext context;

        public RetrieverToRepository(RetrieverContext context)
        {
            this.context = context;
        }

        public override void OnWritten(FileHash hash, RepositoryBlock block)
        {
            context.Omnibus.Complete(new OmnibusBlock(block.Piece, block.Offset, block.Size));
        }

        public override void OnAccepted(FileHash hash, PieceInfo piece)
        {
            context.Omnibus.Complete(piece.Index);
            context.Callback.OnPieceVerified(hash, piece);
        }

        public override void OnRejected(FileHash hash, PieceInfo piece)
        {
            context.Omnibus.Invalidate(piece.Index);
        }
    }
}