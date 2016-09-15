using Leak.Core.Metadata;
using System.IO;

namespace Leak.Core.Repository
{
    public class RepositoryTaskAllocate : RepositoryTask
    {
        public void Accept(RepositoryTaskVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Execute(RepositoryContext context)
        {
            long position = 0;
            int pieceSize = context.Metainfo.Properties.PieceSize;
            int pieces = context.Metainfo.Properties.Pieces;

            RepositoryAllocation allocation = new RepositoryAllocation(pieces);

            foreach (MetainfoEntry entry in context.Metainfo.Entries)
            {
                string path = entry.GetPath(context.Destination);
                FileInfo file = new FileInfo(path);

                if (file.Exists == false)
                {
                    EnsureDirectoryExists(path);
                }
                else
                {
                    allocation.Add(entry, new RepositoryAllocationRange((int)(position / pieceSize), (int)((position + entry.Size) / pieceSize)));
                }

                using (FileStream stream = file.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    if (stream.Length != entry.Size)
                    {
                        stream.SetLength(entry.Size);
                        stream.Flush();
                    }
                }

                position += entry.Size;
            }

            context.Callback.OnAllocated(context.Metainfo.Hash, allocation);
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