using Leak.Core.Common;

namespace Leak.Core.Retriever
{
    public abstract class RetrieverCallbackBase : RetrieverCallback
    {
        public virtual void OnFileStarted(FileHash hash)
        {
        }

        public virtual void OnFileChanged(FileHash hash, BitfieldInfo bitfield)
        {
        }

        public virtual void OnFileCompleted(FileHash hash)
        {
        }

        public virtual void OnPieceVerified(FileHash hash, PieceInfo piece)
        {
        }

        public virtual void OnPieceRejected(FileHash hash, PieceInfo piece)
        {
        }
    }
}