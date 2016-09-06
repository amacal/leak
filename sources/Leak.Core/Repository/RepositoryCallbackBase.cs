using Leak.Core.Common;

namespace Leak.Core.Repository
{
    public abstract class RepositoryCallbackBase : RepositoryCallback
    {
        public virtual void OnAllocated(FileHash hash)
        {
        }

        public virtual void OnVerified(FileHash hash, Bitfield bitfield)
        {
        }

        public virtual void OnAccepted(FileHash hash, PieceInfo piece)
        {
        }

        public virtual void OnRejected(FileHash hash, PieceInfo piece)
        {
        }

        public virtual void OnWritten(FileHash hash, RepositoryBlock block)
        {
        }
    }
}