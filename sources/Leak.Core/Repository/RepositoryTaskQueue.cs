using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Repository
{
    public class RepositoryTaskQueue
    {
        private readonly ConcurrentQueue<RepositoryTask> ready;
        private readonly ConcurrentQueue<RepositoryTask> items;

        public RepositoryTaskQueue()
        {
            ready = new ConcurrentQueue<RepositoryTask>();
            items = new ConcurrentQueue<RepositoryTask>();
        }

        public void Add(RepositoryTask task)
        {
            items.Enqueue(task);
        }

        public void Clear()
        {
            RepositoryTask task;

            while (items.TryDequeue(out task))
            {
            }
        }

        public void Process(RepositoryContext context)
        {
            RepositoryTask task;
            Merge merge = new Merge(context);

            while (items.TryDequeue(out task))
            {
                task.Accept(merge);
            }

            merge.Handle(ready);

            while (ready.TryDequeue(out task))
            {
                task.Execute(context);
            }
        }

        private class Merge : RepositoryTaskVisitor
        {
            private readonly RepositoryContext context;

            private readonly List<RepositoryTask> before;
            private readonly List<RepositoryTaskWriteBlock> writes;
            private readonly List<RepositoryTask> after;

            public Merge(RepositoryContext context)
            {
                this.context = context;

                this.before = new List<RepositoryTask>();
                this.writes = new List<RepositoryTaskWriteBlock>();
                this.after = new List<RepositoryTask>();
            }

            public void Visit(RepositoryTaskAllocate task)
            {
                before.Add(task);
            }

            public void Visit(RepositoryTaskVerifyPiece task)
            {
                after.Add(task);
            }

            public void Visit(RepositoryTaskVerifyRange task)
            {
                before.Add(task);
            }

            public void Visit(RepositoryTaskWriteBlock task)
            {
                writes.Add(task);
            }

            public void Handle(ConcurrentQueue<RepositoryTask> tasks)
            {
                foreach (RepositoryTask task in before)
                {
                    tasks.Enqueue(task);
                }

                foreach (var byPiece in writes.GroupBy(x => x.Piece))
                {
                    List<RepositoryBlockData> blocks = new List<RepositoryBlockData>();

                    foreach (var task in byPiece)
                    {
                        task.MergeInto(blocks);
                    }

                    foreach (var batch in Batch(blocks))
                    {
                        tasks.Enqueue(new RepositoryTaskWriteData(batch));
                    }
                }

                foreach (RepositoryTask task in after)
                {
                    tasks.Enqueue(task);
                }
            }

            private IEnumerable<RepositoryBlockData[]> Batch(IEnumerable<RepositoryBlockData> blocks)
            {
                int? next = null;
                int size = context.Metainfo.Properties.BlockSize;
                List<RepositoryBlockData> result = new List<RepositoryBlockData>();

                foreach (RepositoryBlockData block in blocks.OrderBy(x => x.Offset))
                {
                    if (next == null || next == block.Offset)
                    {
                        result.Add(block);
                        next = block.Offset + size;
                    }
                    else if (next != null && next > block.Offset)
                    {
                    }
                    else if (result.Count > 0)
                    {
                        yield return result.ToArray();

                        result.Clear();
                        result.Add(block);
                        next = block.Offset + size;
                    }
                }

                if (result.Count > 0)
                {
                    yield return result.ToArray();
                }
            }
        }
    }
}