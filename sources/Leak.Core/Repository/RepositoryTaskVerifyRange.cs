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
            Bitfield bitfield = context.Bitfile.Read();

            bitfield = bitfield ?? new Bitfield(length);
            Bitfield reduced = ReduceScope(bitfield);
            int next = Next(reduced, 0);

            if (next < length)
            {
                context.Queue.Add(new Start(bitfield, reduced, next));
            }
            else
            {
                onCompleted.Invoke(this);
                context.Callback.OnVerified(context.Metainfo.Hash, bitfield);
            }
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
            queue.Release("all");
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

        private static int Next(Bitfield scope, int index)
        {
            while (index < scope.Length && scope[index])
            {
                index++;
            }

            return index;
        }

        private class Start : RepositoryTask
        {
            private readonly HashAlgorithm algorithm;
            private readonly Bitfield bitfield;
            private readonly Bitfield scope;
            private readonly int piece;

            public Start(Bitfield bitfield, Bitfield scope, int piece)
            {
                this.algorithm = SHA1.Create();

                this.bitfield = bitfield;
                this.scope = scope;
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
                        context.Queue.Add(new Continue(bitfield, scope, algorithm, args));
                    }
                    else
                    {
                        context.Queue.Add(new Complete(bitfield, scope, algorithm, args));
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
            private readonly Bitfield scope;
            private readonly HashAlgorithm algorithm;
            private readonly RepositoryViewRead read;

            public Continue(Bitfield bitfield, Bitfield scope, HashAlgorithm algorithm, RepositoryViewRead read)
            {
                this.bitfield = bitfield;
                this.scope = scope;
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
                        context.Queue.Add(new Continue(bitfield, scope, algorithm, args));
                    }
                    else
                    {
                        context.Queue.Add(new Complete(bitfield, scope, algorithm, args));
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
            private readonly Bitfield scope;
            private readonly HashAlgorithm algorithm;
            private readonly RepositoryViewRead read;

            public Complete(Bitfield bitfield, Bitfield scope, HashAlgorithm algorithm, RepositoryViewRead read)
            {
                this.bitfield = bitfield;
                this.scope = scope;
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

                int next = Next(scope, read.Piece + 1);
                bool exists = context.View.Exists(next, 0);

                if (exists)
                {
                    context.Queue.Add(new Start(bitfield, scope, next));
                }
                else
                {
                    onCompleted.Invoke(this);
                    context.Bitfile.Write(bitfield);
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