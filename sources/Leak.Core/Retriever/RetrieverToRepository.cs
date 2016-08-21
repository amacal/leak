using Leak.Core.Common;
using Leak.Core.Repository;

namespace Leak.Core.Retriever
{
    public class RetrieverToRepository : RepositoryCallbackBase
    {
        private readonly RetrieverContext context;

        public RetrieverToRepository(RetrieverContext context)
        {
            this.context = context;
        }

        public override void OnAccepted(FileHash hash, RepositoryPiece piece)
        {
            context.Callback.OnPieceVerified(hash, new RetrieverPiece(piece.Index));
        }

        public override void OnRejected(FileHash hash, RepositoryPiece piece)
        {
            context.Omnibus.Invalidate(piece.Index);
        }
    }
}