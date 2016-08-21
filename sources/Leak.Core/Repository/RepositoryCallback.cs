using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Repository
{
    public interface RepositoryCallback
    {
        void OnAllocated(FileHash hash);

        void OnVerified(FileHash hash, Bitfield bitfield);

        void OnAccepted(FileHash hash, RepositoryPiece piece);

        void OnRejected(FileHash hash, RepositoryPiece piece);

        void OnWritten(FileHash hash, RepositoryBlock block);
    }
}