using Leak.Common;
using Leak.Files;

namespace Leak.Data.Store
{
    public class RepositoryViewAllocator
    {
        private readonly FileFactory factory;

        public RepositoryViewAllocator(FileFactory factory)
        {
            this.factory = factory;
        }

        public RepositoryViewCache Allocate(string destination, MetainfoEntry[] entries, int pieceSize, int blockSize)
        {
            long position = 0;
            RepositoryViewEntry[] data = new RepositoryViewEntry[entries.Length];

            for (int i = 0; i < entries.Length; i++)
            {
                data[i] = new RepositoryViewEntry
                {
                    Start = position,
                    End = position + entries[i].Size,
                    Size = entries[i].Size,
                    File = factory.OpenOrCreate(entries[i].GetPath(destination))
                };

                data[i].File.SetLength(entries[i].Size);
                position = position + entries[i].Size;
            }

            return new RepositoryViewCache(data, pieceSize, blockSize);
        }
    }
}