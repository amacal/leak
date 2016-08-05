using Leak.Core.Messages;
using Leak.Core.Metadata;
using System.IO;
using System.Security.Cryptography;

namespace Leak.Core.Repository
{
    public class ResourceRepositoryToMetainfo : ResourceRepository
    {
        private readonly Metainfo metainfo;
        private readonly string location;
        private readonly byte[] buffer;

        public ResourceRepositoryToMetainfo(Metainfo metainfo, string location)
        {
            this.metainfo = metainfo;
            this.location = location;

            this.buffer = new byte[metainfo.Properties.PieceSize];
        }

        public MetainfoProperties Properties
        {
            get { return metainfo.Properties; }
        }

        public Bitfield Initialize()
        {
            foreach (MetainfoEntry entry in metainfo.Entries)
            {
                string path = GetEntryPath(location, entry);

                ResourceRepositoryStream.EnsureDirectory(path);
                FileInfo file = new FileInfo(path);

                using (FileStream stream = file.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    stream.SetLength(entry.Size);
                    stream.Flush();
                }
            }

            return Verify();
        }

        public ResourceRepository WithMetainfo(out Metainfo metainfo)
        {
            metainfo = this.metainfo;
            return this;
        }

        public ResourceRepositorySession OpenSession()
        {
            return new ResourceRepositoryToMetainfoSession(new ResourceRepositoryStream(location, metainfo), metainfo, buffer);
        }

        private Bitfield Verify()
        {
            byte[] hash;
            int piece = 0;

            using (HashAlgorithm algorithm = SHA1.Create())
            using (ResourceRepositoryStream stream = new ResourceRepositoryStream(location, metainfo))
            {
                int read = metainfo.Properties.PieceSize;
                Bitfield bitfield = new Bitfield(metainfo.Properties.Pieces);

                stream.Seek(0, SeekOrigin.Begin);

                while (piece < metainfo.Properties.Pieces)
                {
                    read = stream.Read(buffer, 0, read);
                    hash = algorithm.ComputeHash(buffer, 0, read);

                    if (Bytes.Equals(hash, metainfo.Pieces[piece].ToBytes()))
                    {
                        bitfield[piece] = true;
                    }

                    piece = piece + 1;
                }

                return bitfield;
            }
        }

        private static string GetEntryPath(string location, MetainfoEntry entry)
        {
            return ResourceRepositoryStream.GetEntryPath(location, entry);
        }
    }
}