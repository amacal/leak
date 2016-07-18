using Leak.Core.Metadata;
using System;
using System.IO;

namespace Leak.Core.Repository
{
    public class ResourceRepository
    {
        private readonly Metainfo metainfo;
        private readonly string location;

        public ResourceRepository(Metainfo metainfo, string location)
        {
            this.metainfo = metainfo;
            this.location = location;
        }

        public void Initialize()
        {
            foreach (MetainfoEntry entry in metainfo.Entries)
            {
                string path = GetEntryPath(location, entry);
                FileInfo file = new FileInfo(path);

                using (FileStream stream = file.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    stream.SetLength(entry.Size);
                    stream.Flush();
                }
            }
        }

        private static string GetEntryPath(string location, MetainfoEntry entry)
        {
            string path = String.Join(Path.DirectorySeparatorChar.ToString(), entry.Name);
            string result = Path.Combine(location, path);

            return result;
        }

        public void SetPiece(int piece, int block, byte[] data)
        {
        }
    }
}