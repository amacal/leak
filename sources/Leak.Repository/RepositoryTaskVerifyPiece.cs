using System;
using System.Security.Cryptography;
using Leak.Common;

namespace Leak.Data.Store
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
            int bufferSize = context.Configuration.BufferSize;

            RepositoryMemoryBlock block = context.Dependencies.Memory.Allocate(bufferSize);
            int step = block.Length / context.Metainfo.Properties.BlockSize;

            context.View.Read(block.Data, piece.Index, 0, args =>
            {
                if (args.Count > 0 && context.View.Exists(piece.Index, args.Block + step))
                {
                    context.Queue.Add(new Continue(piece, args, algorithm, block));
                }
                else
                {
                    context.Queue.Add(new Complete(piece, args, algorithm, block));
                }
            });
        }

        public bool CanExecute(RepositoryTaskQueue queue)
        {
            return queue.IsBlocked(piece) == false;
        }

        public void Block(RepositoryTaskQueue queue)
        {
            queue.Block(piece);
        }

        public void Release(RepositoryTaskQueue queue)
        {
        }

        private class Continue : RepositoryTask
        {
            private readonly PieceInfo piece;
            private readonly RepositoryViewRead read;
            private readonly HashAlgorithm algorithm;
            private readonly RepositoryMemoryBlock block;

            public Continue(PieceInfo piece, RepositoryViewRead read, HashAlgorithm algorithm, RepositoryMemoryBlock block)
            {
                this.piece = piece;
                this.read = read;
                this.algorithm = algorithm;
                this.block = block;
            }

            public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
            {
                algorithm.Push(read.Buffer.Data, read.Buffer.Offset, Math.Min(read.Buffer.Count, read.Count));
                int step = block.Length / context.Metainfo.Properties.BlockSize;

                context.View.Read(block.Data, piece.Index, read.Block + step, args =>
                {
                    if (args.Count > 0 && context.View.Exists(piece.Index, args.Block + step))
                    {
                        context.Queue.Add(new Continue(piece, args, algorithm, block));
                    }
                    else
                    {
                        context.Queue.Add(new Complete(piece, args, algorithm, block));
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
            private readonly RepositoryMemoryBlock block;

            public Complete(PieceInfo piece, RepositoryViewRead read, HashAlgorithm algorithm, RepositoryMemoryBlock block)
            {
                this.piece = piece;
                this.read = read;
                this.algorithm = algorithm;
                this.block = block;
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
                block.Release();

                onCompleted.Invoke(this);
            }

            public void Block(RepositoryTaskQueue queue)
            {
            }

            public void Release(RepositoryTaskQueue queue)
            {
                queue.Release(piece);
            }

            private void AcceptIfRequired(RepositoryContext context, bool valid)
            {
                if (valid)
                {
                    context.Hooks.CallPieceAccepted(context.Metainfo.Hash, piece);
                }
            }

            private void RejectIfRequired(RepositoryContext context, bool valid)
            {
                if (valid == false)
                {
                    context.Hooks.CallPieceRejected(context.Metainfo.Hash, piece);
                }
            }
        }
    }
}