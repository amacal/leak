using Leak.Common;
using Leak.Files;

namespace Leak.Repository
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
            byte[] buffer = new byte[blockSize];

            context.View.Read(buffer, index.Piece, index.Offset / blockSize, result =>
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
                context.Hooks.CallBlockRead(context.Parameters.Hash, index, new FixedDataBlock(read.Buffer, read.Count));
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
        }

        public class FixedDataBlock : DataBlock
        {
            private readonly FileBuffer buffer;
            private readonly int count;
            private readonly int offset;

            public FixedDataBlock(FileBuffer buffer, int count)
            {
                this.buffer = buffer;
                this.count = count;
            }

            private FixedDataBlock(FileBuffer buffer, int offset, int count)
            {
                this.buffer = buffer;
                this.offset = offset;
                this.count = count;
            }

            public int Size
            {
                get { return count - offset; }
            }

            public byte this[int index]
            {
                get { return buffer.Data[buffer.Offset + index + offset]; }
            }

            public void Write(DataBlockCallback callback)
            {
                callback.Invoke(buffer.Data, buffer.Offset + offset, count - offset);
            }

            public DataBlock Scope(int shift)
            {
                return new FixedDataBlock(buffer, shift + offset, count);
            }

            public void Dispose()
            {
            }
        }
    }
}
