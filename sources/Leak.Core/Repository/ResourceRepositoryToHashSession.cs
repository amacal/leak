using Leak.Core.Common;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Leak.Core.Repository
{
    public class ResourceRepositoryToHashSession : ResourceRepositorySession
    {
        private readonly FileHash hash;
        private readonly string location;

        public ResourceRepositoryToHashSession(FileHash hash, string location)
        {
            this.hash = hash;
            this.location = location;
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
            throw new NotSupportedException();
        }

        public void Dispose()
        {
        }
    }
}