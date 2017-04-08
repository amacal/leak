using System;
using System.Diagnostics;
using Leak.Common;
using Leak.Files;
using Leak.Networking.Core;

namespace Leak.Data.Store
{
    public class RepositoryTaskReadBlock : RepositoryTask
    {
        private readonly BlockIndex index;

        public RepositoryTaskReadBlock(BlockIndex index)
        {
            this.index = index;
        }

        public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
        {
            int blockSize = context.Metainfo.Properties.BlockSize;
            RepositoryMemoryBlock block = context.Dependencies.Memory.Allocate(blockSize);
            FileBuffer buffer = new FileBuffer(block.Data, 0, blockSize);

            context.View.Read(buffer, index.Piece.Index, index.Offset / blockSize, result =>
            {
                context.Queue.Add(new Complete(index, result));
            });
        }

        public bool CanExecute(RepositoryTaskQueue queue)
        {
            return queue.IsBlocked(index.Piece) == false;
        }

        public void Block(RepositoryTaskQueue queue)
        {
            queue.Block(index.Piece);
        }

        public void Release(RepositoryTaskQueue queue)
        {
        }

        private class Complete : RepositoryTask
        {
            private readonly BlockIndex index;
            private readonly RepositoryViewRead read;

            public Complete(BlockIndex index, RepositoryViewRead read)
            {
                this.index = index;
                this.read = read;
            }

            public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
            {
                onCompleted.Invoke(this);

                Action<byte[]> release = context.Dependencies.Memory.Release;
                DataBlock data = new FileBufferDataBlock(read, release);

                context.Hooks.CallBlockRead(context.Parameters.Hash, index, data);
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
                queue.Release(index.Piece);
            }

            private class FileBufferDataBlock : DataBlock
            {
                private readonly RepositoryViewRead read;
                private readonly Action<byte[]> release;
                private readonly int offset;

                public FileBufferDataBlock(RepositoryViewRead read, Action<byte[]> release)
                {
                    this.read = read;
                    this.release = release;
                    this.offset = 0;
                }

                private FileBufferDataBlock(RepositoryViewRead read, Action<byte[]> release, int offset)
                {
                    this.read = read;
                    this.release = release;
                    this.offset = offset;
                }

                public int Length
                {
                    get { return read.Count; }
                }

                public void With(DataBlockCallback callback)
                {
                    callback.Invoke(read.Buffer.Data, offset, read.Count - offset);
                }

                public DataBlock Scope(int shift)
                {
                    return new FileBufferDataBlock(read, release, offset + shift);
                }

                public void Release()
                {
                    release.Invoke(read.Buffer.Data);
                }
            }
        }
    }
}