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

        public void Read(byte[] buffer, int piece, RepositoryViewReadCallback callback)
        {
            long offset = piece * (long)cache.PieceSize;
            RepositoryViewEntry[] entries = cache.Find(piece);
            RepositoryViewReadRoutine routine = new RepositoryViewReadRoutine(piece, entries, buffer, offset, callback);

            routine.Execute();
        }

        public void Write(FileBuffer buffer, int piece, int block, RepositoryViewWriteCallback callback)
        {
            long offset = piece * (long)cache.PieceSize + block * cache.BlockSize;
            RepositoryViewEntry[] entries = cache.Find(piece);
            RepositoryViewWriteRoutine routine = new RepositoryViewWriteRoutine(piece, entries, buffer, offset, callback);

            routine.Execute();
        }
    }
}