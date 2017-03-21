using System;
using System.Security.Cryptography;
using Leak.Common;
using Leak.Files;

namespace Leak.Data.Store
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
                int bufferSize = context.Configuration.BufferSize;
                RepositoryMemoryBlock block = context.Dependencies.Memory.Allocate(bufferSize);

                context.Queue.Add(new Start(bitfield, reduced, next, block));
            }
            else
            {
                onCompleted.Invoke(this);
                context.Hooks.CallDataVerified(context.Metainfo.Hash, bitfield);
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
            private readonly RepositoryMemoryBlock block;

            public Start(Bitfield bitfield, Bitfield scope, int piece, RepositoryMemoryBlock block)
            {
                this.algorithm = SHA1.Create();

                this.bitfield = bitfield;
                this.scope = scope;
                this.piece = piece;
                this.block = block;
            }

            public bool CanExecute(RepositoryTaskQueue queue)
            {
                return true;
            }

            public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
            {
                int blocksInBuffer = block.Length / context.Metainfo.Properties.BlockSize;
                int blocksInPiece = context.Metainfo.Properties.PieceSize / context.Metainfo.Properties.BlockSize;

                int step = Math.Min(blocksInBuffer, blocksInPiece);
                FileBuffer buffer = new FileBuffer(block.Data, 0, step * context.Metainfo.Properties.BlockSize);

                context.View.Read(buffer, piece, 0, args =>
                {
                    if (args.Count > 0 && context.View.Exists(args.Piece, args.Block + step))
                    {
                        context.Queue.Add(new Continue(bitfield, scope, algorithm, args, block));
                    }
                    else
                    {
                        context.Queue.Add(new Complete(bitfield, scope, algorithm, args, block));
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
            private readonly RepositoryMemoryBlock block;

            public Continue(Bitfield bitfield, Bitfield scope, HashAlgorithm algorithm, RepositoryViewRead read, RepositoryMemoryBlock block)
            {
                this.bitfield = bitfield;
                this.scope = scope;
                this.algorithm = algorithm;
                this.read = read;
                this.block = block;
            }

            public bool CanExecute(RepositoryTaskQueue queue)
            {
                return true;
            }

            public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
            {
                algorithm.Push(read.Buffer.Data, read.Buffer.Offset, Math.Min(read.Buffer.Count, read.Count));
                int step = block.Length / context.Metainfo.Properties.BlockSize;

                context.View.Read(block.Data, read.Piece, read.Block + step, args =>
                {
                    if (args.Count > 0 && context.View.Exists(args.Piece, args.Block + step))
                    {
                        context.Queue.Add(new Continue(bitfield, scope, algorithm, args, block));
                    }
                    else
                    {
                        context.Queue.Add(new Complete(bitfield, scope, algorithm, args, block));
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
            private readonly RepositoryMemoryBlock block;

            public Complete(Bitfield bitfield, Bitfield scope, HashAlgorithm algorithm, RepositoryViewRead read, RepositoryMemoryBlock block)
            {
                this.bitfield = bitfield;
                this.scope = scope;
                this.algorithm = algorithm;
                this.read = read;
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
                byte[] expected = metainfo.Pieces[read.Piece].ToBytes();

                byte[] hash = algorithm.Complete();
                bool result = Bytes.Equals(hash, expected);

                bitfield[read.Piece] = result;
                algorithm.Dispose();

                int next = Next(scope, read.Piece + 1);
                bool exists = context.View.Exists(next, 0);

                if (exists)
                {
                    context.Queue.Add(new Start(bitfield, scope, next, block));
                }
                else
                {
                    block.Release();
                    onCompleted.Invoke(this);

                    context.Bitfile.Write(bitfield);
                    context.Hooks.CallDataVerified(metainfo.Hash, bitfield);
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