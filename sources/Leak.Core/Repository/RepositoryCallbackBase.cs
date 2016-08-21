using Leak.Core.Common;
using Leak.Core.Messages;

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

        public virtual void OnAccepted(FileHash hash, RepositoryPiece piece)
        {
        }

        public virtual void OnRejected(FileHash hash, RepositoryPiece piece)
        {
        }

        public virtual void OnWritten(FileHash hash, RepositoryBlock block)
        {
        }
    }
}