using System.IO;
using Leak.Common;

namespace Leak.Data.Store
{
    public class RepositoryTaskAllocate : RepositoryTask
    {
        public bool CanExecute(RepositoryTaskQueue queue)
        {
            return true;
        }

        public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
        {
            long position = 0;
            int pieces = context.Metainfo.Properties.Pieces;

            int pieceSize = context.Metainfo.Properties.PieceSize;
            int blockSize = context.Metainfo.Properties.BlockSize;

            RepositoryAllocation allocation = new RepositoryAllocation(pieces);
            RepositoryViewAllocator allocator = new RepositoryViewAllocator(context.Dependencies.Files);

            foreach (MetainfoEntry entry in context.Metainfo.Entries)
            {
                string path = entry.GetPath(context.Parameters.Destination);
                FileInfo info = new FileInfo(path);

                if (info.Exists == false)
                {
                    EnsureDirectoryExists(path);
                    allocation.Add(entry, new RepositoryAllocationRange((int)(position / pieceSize), (int)((position + entry.Size) / pieceSize)));
                }

                position += entry.Size;
            }

            MetainfoEntry[] entries = context.Metainfo.Entries;
            RepositoryViewCache cache = allocator.Allocate(context.Parameters.Destination, entries, pieceSize, blockSize);

            context.View = new RepositoryView(cache);
            context.Hooks.CallDataAllocated(context.Metainfo.Hash, context.Parameters.Destination);
        }

        public void Block(RepositoryTaskQueue queue)
        {
        }

        public void Release(RepositoryTaskQueue queue)
        {
        }

        private static void EnsureDirectoryExists(string path)
        {
            string directory = Path.GetDirectoryName(path);

            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}