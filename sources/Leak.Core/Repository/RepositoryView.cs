using Leak.Files;

namespace Leak.Core.Repository
{
    public class RepositoryView
    {
        private readonly RepositoryViewCache cache;

        public RepositoryView(RepositoryViewCache cache)
        {
            this.cache = cache;
        }

        public int BlocksPerPiece
        {
            get { return cache.PieceSize / cache.BlockSize; }
        }

        public void Read(byte[] buffer, int piece, RepositoryViewReadCallback callback)
        {
            new RepositoryViewReadRoutine(cache, piece, buffer, callback).Execute();
        }

        public void Read(byte[] buffer, int piece, int block, RepositoryViewReadCallback callback)
        {
            new RepositoryViewReadRoutine(cache, piece, block, buffer, callback).Execute();
        }

        public void Write(FileBuffer buffer, int piece, int block, RepositoryViewWriteCallback callback)
        {
            new RepositoryViewWriteRoutine(cache, piece, block, buffer, callback).Execute();
        }

        public void Flush()
        {
            cache.Flush();
        }
    }
}