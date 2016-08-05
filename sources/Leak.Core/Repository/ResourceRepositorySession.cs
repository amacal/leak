using System;

namespace Leak.Core.Repository
{
    public interface ResourceRepositorySession : IDisposable
    {
        void SetPiece(int piece, int block, byte[] data);

        bool SetMetadata(int piece, byte[] data);

        bool Verify(int piece);
    }
}