using System.Collections.Concurrent;
using System.Collections.Generic;
using Leak.Tasks;

namespace Leak.Core.Repository
{
    public class RepositoryTaskQueue : LeakQueueBase<RepositoryContext>
    {
        private readonly ConcurrentQueue<RepositoryTask> ready;
        private readonly ConcurrentQueue<RepositoryTask> items;
        private readonly HashSet<object> keys;

        public RepositoryTaskQueue()
        {
            ready = new ConcurrentQueue<RepositoryTask>();
            items = new ConcurrentQueue<RepositoryTask>();
            keys = new HashSet<object>();
        }

        public void Add(RepositoryTask task)
        {
            items.Enqueue(task);
            onReady.Set();
        }

        public bool IsBlocked(object key)
        {
            return keys.Contains(key);
        }

        public void Block(object key)
        {
            keys.Add(key);
        }

        public void Release(object key)
        {
            keys.Remove(key);
        }

        protected override void OnProcess(RepositoryContext context)
        {
            RepositoryTask task;

            while (items.TryDequeue(out task))
            {
                ready.Enqueue(task);
            }

            int count = ready.Count;

            while (count-- > 0)
            {
                ready.TryDequeue(out task);

                if (task.CanExecute(this))
                {
                    task.Block(this);
                    task.Execute(context, OnCompleted);
                }
                else
                {
                    ready.Enqueue(task);
                }
            }
        }

        private void OnCompleted(RepositoryTask task)
        {
            task.Release(this);
            onReady.Set();
        }
    }
}