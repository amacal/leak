using Leak.Core.Metadata;
using System.IO;
using System.Security.Cryptography;

namespace Leak.Core.Repository
{
    public class ResourceRepositoryToMetainfoSession : ResourceRepositorySession
    {
        private readonly ResourceRepositoryStream stream;
        private readonly Metainfo metainfo;
        private readonly byte[] buffer;

        public ResourceRepositoryToMetainfoSession(ResourceRepositoryStream stream, Metainfo metainfo, byte[] buffer)
        {
            this.stream = stream;
            this.metainfo = metainfo;
            this.buffer = buffer;
        }

        public void SetPiece(int piece, int block, byte[] data)
        {
            int pieceSize = metainfo.Properties.PieceSize;
            int blockSize = metainfo.Properties.BlockSize;
            long position = (long)piece * pieceSize + block * blockSize;

            stream.Seek(position, SeekOrigin.Begin);
            stream.Write(data, 0, data.Length);
        }

        public bool SetMetadata(int piece, byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public bool Verify(int piece)
        {
            long position = (long)piece * metainfo.Properties.PieceSize;

            using (HashAlgorithm algorithm = SHA1.Create())
            {
                stream.Seek(position, SeekOrigin.Begin);

                int read = stream.Read(buffer, 0, buffer.Length);
                byte[] hash = algorithm.ComputeHash(buffer, 0, read);

                return Bytes.Equals(hash, metainfo.Pieces[piece].ToBytes());
            }
        }

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}