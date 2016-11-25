using Leak.Files;

namespace Leak.Core.Repository
{
    public class RepositoryTaskWriteBlock : RepositoryTask
    {
        private readonly RepositoryBlockData data;

        public RepositoryTaskWriteBlock(RepositoryBlockData data)
        {
            this.data = data;
        }

        public int Piece
        {
            get { return data.Piece; }
        }

        public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
        {
            data.Write((buffer, offset, count) =>
            {
                int blockSize = context.Metainfo.Properties.BlockSize;
                FileBuffer file = new FileBuffer(buffer, offset, count);

                context.View.Write(file, data.Piece, data.Offset / blockSize, args =>
                {
                    context.Queue.Add(new Complete(data));
                });
            });
        }

        public bool CanExecute(RepositoryTaskQueue queue)
        {
            return queue.IsBlocked(data.Piece) == false;
        }

        public void Block(RepositoryTaskQueue queue)
        {
            queue.Block(data.Piece);
        }

        public void Release(RepositoryTaskQueue queue)
        {
        }

        private class Complete : RepositoryTask
        {
            private readonly RepositoryBlockData data;

            public Complete(RepositoryBlockData data)
            {
                this.data = data;
            }

            public bool CanExecute(RepositoryTaskQueue queue)
            {
                return true;
            }

            public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
            {
                onCompleted.Invoke(this);
                //context.Callback.OnWritten(context.Metainfo.Hash, data.ToBlock());
                data.Dispose();
            }

            public void Block(RepositoryTaskQueue queue)
            {
            }

            public void Release(RepositoryTaskQueue queue)
            {
                queue.Release(data.Piece);
            }
        }
    }
}