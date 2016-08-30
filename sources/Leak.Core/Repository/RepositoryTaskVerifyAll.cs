using Leak.Core.Messages;
using Leak.Core.Metadata;
using System.IO;
using System.Security.Cryptography;
using Leak.Core.Common;

namespace Leak.Core.Repository
{
    public class RepositoryTaskVerifyAll : RepositoryTask
    {
        public void Execute(RepositoryContext context)
        {
            byte[] hash;
            int piece = 0;

            Metainfo metainfo = context.Metainfo;
            string destination = context.Destination;

            using (HashAlgorithm algorithm = SHA1.Create())
            using (RepositoryStream stream = new RepositoryStream(destination, metainfo))
            {
                int read = metainfo.Properties.PieceSize;
                Bitfield bitfield = new Bitfield(metainfo.Properties.Pieces);

                stream.Seek(0, SeekOrigin.Begin);

                while (piece < metainfo.Properties.Pieces)
                {
                    read = stream.Read(context.Buffer, 0, read);
                    hash = algorithm.ComputeHash(context.Buffer, 0, read);

                    if (Bytes.Equals(hash, metainfo.Pieces[piece].ToBytes()))
                    {
                        bitfield[piece] = true;
                    }

                    piece = piece + 1;
                }

                context.Callback.OnVerified(metainfo.Hash, bitfield);
            }
        }
    }
}