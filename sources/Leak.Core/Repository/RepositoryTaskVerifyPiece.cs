using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Metadata;
using System.IO;
using System.Security.Cryptography;

namespace Leak.Core.Repository
{
    public class RepositoryTaskVerifyPiece : LeakTask<RepositoryContext>
    {
        private readonly PieceInfo piece;

        public RepositoryTaskVerifyPiece(PieceInfo piece)
        {
            this.piece = piece;
        }

        public void Execute(RepositoryContext context)
        {
            Metainfo metainfo = context.Metainfo;
            string destination = context.Destination;

            bool result;
            long position = (long)piece.Index * metainfo.Properties.PieceSize;

            using (HashAlgorithm algorithm = SHA1.Create())
            using (RepositoryStream stream = new RepositoryStream(destination, metainfo))
            {
                stream.Seek(position, SeekOrigin.Begin);

                int read = stream.Read(context.Buffer, 0, context.Buffer.Length);
                byte[] hash = algorithm.ComputeHash(context.Buffer, 0, read);

                result = Bytes.Equals(hash, metainfo.Pieces[piece.Index].ToBytes());
            }

            AcceptIfRequired(context, result);
            RejectIfRequired(context, result);
        }

        private void AcceptIfRequired(RepositoryContext context, bool valid)
        {
            if (valid)
            {
                context.Callback.OnAccepted(context.Metainfo.Hash, piece);
            }
        }

        private void RejectIfRequired(RepositoryContext context, bool valid)
        {
            if (valid == false)
            {
                context.Callback.OnRejected(context.Metainfo.Hash, piece);
            }
        }
    }
}