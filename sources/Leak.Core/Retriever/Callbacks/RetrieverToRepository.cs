using Leak.Core.Common;
using Leak.Core.Omnibus;
using Leak.Core.Repository;

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

        public override void OnAccepted(FileHash hash, RepositoryPiece piece)
        {
            context.Omnibus.Complete(piece.Index);
            context.Callback.OnVerified(hash, new RetrieverPiece(piece.Index));
        }

        public override void OnRejected(FileHash hash, RepositoryPiece piece)
        {
            context.Omnibus.Invalidate(piece.Index);
        }
    }
}