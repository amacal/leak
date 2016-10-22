using Leak.Core.Common;
using Leak.Core.Metadata;
using System.Security.Cryptography;

namespace Leak.Core.Repository
{
    public class RepositoryTaskVerifyPiece : RepositoryTask
    {
        private readonly PieceInfo piece;

        public RepositoryTaskVerifyPiece(PieceInfo piece)
        {
            this.piece = piece;
        }

        public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
        {
            context.View.Read(context.Buffer, piece.Index, data =>
            {
                context.Queue.Add(new Complete(piece, data));
            });
        }

        public bool CanExecute(RepositoryTaskQueue queue)
        {
            return queue.IsBlocked("all") == false;
        }

        public void Block(RepositoryTaskQueue queue)
        {
            queue.Block("all");
        }

        public void Release(RepositoryTaskQueue queue)
        {
        }

        private class Complete : RepositoryTask
        {
            private readonly PieceInfo piece;
            private readonly RepositoryViewRead data;

            public Complete(PieceInfo piece, RepositoryViewRead data)
            {
                this.piece = piece;
                this.data = data;
            }

            public bool CanExecute(RepositoryTaskQueue queue)
            {
                return true;
            }

            public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
            {
                Metainfo metainfo = context.Metainfo;

                using (HashAlgorithm algorithm = SHA1.Create())
                {
                    byte[] hash = algorithm.ComputeHash(context.Buffer, 0, data.Count);
                    bool result = Bytes.Equals(hash, metainfo.Pieces[piece.Index].ToBytes());

                    AcceptIfRequired(context, result);
                    RejectIfRequired(context, result);

                    onCompleted.Invoke(this);
                }
            }

            public void Block(RepositoryTaskQueue queue)
            {
            }

            public void Release(RepositoryTaskQueue queue)
            {
                queue.Release("all");
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
}