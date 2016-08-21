using Leak.Core.Common;

namespace Leak.Core.Retriever
{
    public interface RetrieverCallback
    {
        void OnCompleted(FileHash hash);

        void OnPieceVerified(FileHash hash, RetrieverPiece piece);
    }
}