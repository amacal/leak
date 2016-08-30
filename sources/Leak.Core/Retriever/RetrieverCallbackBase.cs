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

        public virtual void OnVerified(FileHash hash, RetrieverPiece piece)
        {
        }

        public virtual void OnProgress(FileHash hash, BitfieldInfo bitfield)
        {
        }
    }
}