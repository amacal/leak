using Leak.Core.Core;
using Leak.Core.Metadata;
using System.IO;

namespace Leak.Core.Repository
{
    public class RepositoryTaskAllocate : LeakTask<RepositoryContext>
    {
        public void Execute(RepositoryContext context)
        {
            foreach (MetainfoEntry entry in context.Metainfo.Entries)
            {
                string path = entry.GetPath(context.Destination);
                FileInfo file = new FileInfo(path);

                if (file.Exists == false)
                {
                    EnsureDirectoryExists(path);
                }

                using (FileStream stream = file.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    stream.SetLength(entry.Size);
                    stream.Flush();
                }
            }

            context.Callback.OnAllocated(context.Metainfo.Hash);
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