using Leak.Core.Common;
using Leak.Core.Metadata;
using System;
using System.Security.Cryptography;

namespace Leak.Core.Repository
{
    public class RepositoryTaskVerifyRange : RepositoryTask
    {
        private readonly Bitfield scope;

        public RepositoryTaskVerifyRange(Bitfield scope)
        {
            this.scope = scope;
        }

        public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
        {
            int length = context.Metainfo.Pieces.Length;
            int size = context.Metainfo.Properties.PieceSize;

            Bitfield bitfield = context.Bitfile.Read();
            bitfield = bitfield ?? new Bitfield(length);

            Bitfield reduced = ReduceScope(bitfield);
            context.Queue.Add(new Start(bitfield, 0));
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

        private Bitfield ReduceScope(Bitfield bitfield)
        {
            int size = bitfield.Length;
            Bitfield reduced = new Bitfield(size);

            for (int i = 0; i < size; i++)
            {
                bitfield[i] = bitfield[i] && scope[i] == false;
                reduced[i] = scope[i] || bitfield[i];
            }

            return reduced;
        }

        private class Start : RepositoryTask
        {
            private readonly HashAlgorithm algorithm;
            private readonly Bitfield bitfield;
            private readonly int piece;

            public Start(Bitfield bitfield, int piece)
            {
                this.algorithm = SHA1.Create();
                this.bitfield = bitfield;
                this.piece = piece;
            }

            public bool CanExecute(RepositoryTaskQueue queue)
            {
                return true;
            }

            public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
            {
                context.View.Read(context.Buffer, piece, 0, args =>
                {
                    if (args.Count > 0 && context.View.Exists(args.Piece, args.Block + 1))
                    {
                        context.Queue.Add(new Continue(bitfield, algorithm, args));
                    }
                    else
                    {
                        context.Queue.Add(new Complete(bitfield, algorithm, args));
                    }
                });
            }

            public void Block(RepositoryTaskQueue queue)
            {
            }

            public void Release(RepositoryTaskQueue queue)
            {
            }
        }

        private class Continue : RepositoryTask
        {
            private readonly Bitfield bitfield;
            private readonly HashAlgorithm algorithm;
            private readonly RepositoryViewRead read;

            public Continue(Bitfield bitfield, HashAlgorithm algorithm, RepositoryViewRead read)
            {
                this.bitfield = bitfield;
                this.algorithm = algorithm;
                this.read = read;
            }

            public bool CanExecute(RepositoryTaskQueue queue)
            {
                return true;
            }

            public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
            {
                algorithm.Push(read.Buffer.Data, read.Buffer.Offset, Math.Min(read.Buffer.Count, read.Count));

                context.View.Read(context.Buffer, read.Piece, read.Block + 1, args =>
                {
                    if (args.Count > 0 && context.View.Exists(args.Piece, args.Block + 1))
                    {
                        context.Queue.Add(new Continue(bitfield, algorithm, args));
                    }
                    else
                    {
                        context.Queue.Add(new Complete(bitfield, algorithm, args));
                    }
                });
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
            private readonly Bitfield bitfield;
            private readonly HashAlgorithm algorithm;
            private readonly RepositoryViewRead read;

            public Complete(Bitfield bitfield, HashAlgorithm algorithm, RepositoryViewRead read)
            {
                this.bitfield = bitfield;
                this.algorithm = algorithm;
                this.read = read;
            }

            public bool CanExecute(RepositoryTaskQueue queue)
            {
                return true;
            }

            public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
            {
                algorithm.Push(read.Buffer.Data, read.Buffer.Offset, Math.Min(read.Buffer.Count, read.Count));

                Metainfo metainfo = context.Metainfo;
                byte[] expected = metainfo.Pieces[read.Piece].ToBytes();

                byte[] hash = algorithm.Complete();
                bool result = Bytes.Equals(hash, expected);

                bitfield[read.Piece] = result;
                algorithm.Dispose();

                if (context.View.Exists(read.Piece + 1, 0))
                {
                    context.Queue.Add(new Start(bitfield, read.Piece + 1));
                }
                else
                {
                    onCompleted.Invoke(this);
                    context.Callback.OnVerified(metainfo.Hash, bitfield);
                }
            }

            public void Block(RepositoryTaskQueue queue)
            {
            }

            public void Release(RepositoryTaskQueue queue)
            {
                queue.Release("all");
            }
        }
    }
}