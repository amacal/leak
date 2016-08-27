using Leak.Core.Common;

namespace Leak.Core.Retriever
{
    public abstract class RetrieverCallbackBase : RetrieverCallback
    {
        public virtual void OnStarted(FileHash hash)
        {
        }

        public virtual void OnCompleted(FileHash hash)
        {
        }

        public virtual void OnPieceVerified(FileHash hash, RetrieverPiece piece)
        {
        }
    }
}