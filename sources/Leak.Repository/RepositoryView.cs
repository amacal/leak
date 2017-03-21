using System;
using Leak.Files;

namespace Leak.Data.Store
{
    public class RepositoryView : IDisposable
    {
        private readonly RepositoryViewCache cache;

        public RepositoryView(RepositoryViewCache cache)
        {
            this.cache = cache;
        }

        public bool Exists(int piece, int block)
        {
            return cache.Exists(piece, block);
        }

        public void Read(FileBuffer buffer, int piece, RepositoryViewReadCallback callback)
        {
            new RepositoryViewReadRoutine(cache, piece, buffer, callback).Execute();
        }

        public void Read(FileBuffer buffer, int piece, int block, RepositoryViewReadCallback callback)
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

        public void Dispose()
        {
            cache.Dispose();
        }
    }
}