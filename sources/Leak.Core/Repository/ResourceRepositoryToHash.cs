using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Metadata;
using System.IO;
using System.Security.Cryptography;

namespace Leak.Core.Repository
{
    public class ResourceRepositoryToHash : ResourceRepository
    {
        private readonly FileHash hash;
        private readonly string location;

        public ResourceRepositoryToHash(FileHash hash, string location)
        {
            this.hash = hash;
            this.location = location;
        }

        public MetainfoProperties Properties
        {
            get { throw new System.NotImplementedException(); }
        }

        public Bitfield Initialize()
        {
            throw new System.NotImplementedException();
        }

        public ResourceRepository WithMetainfo(out Metainfo metainfo)
        {
            string path = Path.Combine(location, $"{hash}.metainfo");
            byte[] data = File.ReadAllBytes(path);

            metainfo = MetainfoFactory.FromBytes(data);
            string destination = Path.Combine(location, hash.ToString());

            return new ResourceRepositoryToMetainfo(metainfo, destination);
        }

        public void SetPiece(int piece, int block, byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public bool SetMetadata(int piece, byte[] data)
        {
            string path = Path.Combine(location, $"{hash}.metainfo");

            using (FileStream file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                file.Seek(piece * 16384, SeekOrigin.Begin);
                file.Write(data, 0, data.Length);
                file.Flush(true);

                file.Seek(0, SeekOrigin.Begin);

                using (HashAlgorithm algorithm = SHA1.Create())
                {
                    return Bytes.Equals(hash.ToBytes(), algorithm.ComputeHash(file));
                }
            }
        }

        public bool Verify(int piece)
        {
            throw new System.NotImplementedException();
        }
    }
}