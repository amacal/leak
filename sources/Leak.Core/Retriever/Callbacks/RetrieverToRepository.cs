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
            OmnibusBlock reference = new OmnibusBlock(block.Piece, block.Offset, block.Size);
            bool completed = context.Omnibus.Complete(reference);

            if (completed)
            {
                context.Repository.Verify(new RepositoryPiece(block.Piece));
            }
        }

        public override void OnAccepted(FileHash hash, RepositoryPiece piece)
        {
            context.Callback.OnVerified(hash, new RetrieverPiece(piece.Index));

            if (context.Omnibus.IsComplete())
            {
                context.Callback.OnCompleted(context.Metainfo.Hash);
            }
        }

        public override void OnRejected(FileHash hash, RepositoryPiece piece)
        {
            context.Omnibus.Invalidate(piece.Index);
        }
    }
}