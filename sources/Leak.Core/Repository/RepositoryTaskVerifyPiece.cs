using Leak.Core.Common;
using Leak.Core.Metadata;
using System;
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
            HashAlgorithm algorithm = SHA1.Create();

            context.View.Read(context.Buffer, piece.Index, 0, args =>
            {
                context.Queue.Add(new Continue(piece, args, algorithm));
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

        private class Continue : RepositoryTask
        {
            private readonly PieceInfo piece;
            private readonly RepositoryViewRead read;
            private readonly HashAlgorithm algorithm;

            public Continue(PieceInfo piece, RepositoryViewRead read, HashAlgorithm algorithm)
            {
                this.piece = piece;
                this.read = read;
                this.algorithm = algorithm;
            }

            public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
            {
                algorithm.Push(read.Buffer.Data, read.Buffer.Offset, Math.Min(read.Buffer.Count, read.Count));

                context.View.Read(context.Buffer, piece.Index, read.Block + 1, args =>
                {
                    if (args.Count > 0 && context.View.Exists(piece.Index, args.Block + 1))
                    {
                        context.Queue.Add(new Continue(piece, args, algorithm));
                    }
                    else
                    {
                        context.Queue.Add(new Complete(piece, args, algorithm));
                    }
                });
            }

            public bool CanExecute(RepositoryTaskQueue queue)
            {
                return true;
            }

            public void Block(RepositoryTaskQueue queue)
            {
            }

            public void Release(RepositoryTaskQueue queue)
            {
            }
        }

        private class Complete : RepositoryTask
        {
            private readonly PieceInfo piece;
            private readonly RepositoryViewRead read;
            private readonly HashAlgorithm algorithm;

            public Complete(PieceInfo piece, RepositoryViewRead read, HashAlgorithm algorithm)
            {
                this.piece = piece;
                this.read = read;
                this.algorithm = algorithm;
            }

            public bool CanExecute(RepositoryTaskQueue queue)
            {
                return true;
            }

            public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
            {
                algorithm.Push(read.Buffer.Data, read.Buffer.Offset, Math.Min(read.Buffer.Count, read.Count));

                Metainfo metainfo = context.Metainfo;
                byte[] expected = metainfo.Pieces[piece.Index].ToBytes();

                byte[] hash = algorithm.Complete();
                bool result = Bytes.Equals(hash, expected);

                AcceptIfRequired(context, result);
                RejectIfRequired(context, result);

                algorithm.Dispose();
                onCompleted.Invoke(this);
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