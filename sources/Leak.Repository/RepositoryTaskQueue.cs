using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Leak.Tasks;

namespace Leak.Data.Store
{
    public class RepositoryTaskQueue : LeakPipelineTrigger
    {
        private readonly RepositoryContext context;
        private readonly Queue<RepositoryTask> ready;
        private readonly ConcurrentQueue<RepositoryTask> items;
        private readonly HashSet<object> keys;

        private ManualResetEvent onReady;
        private bool stopped;

        public RepositoryTaskQueue(RepositoryContext context)
        {
            this.context = context;

            ready = new Queue<RepositoryTask>();
            items = new ConcurrentQueue<RepositoryTask>();
            keys = new HashSet<object>();
        }

        public void Stop()
        {
            stopped = true;
        }

        public void Add(RepositoryTask task)
        {
            if (stopped == false)
            {
                items.Enqueue(task);
                onReady?.Set();
            }
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

        void LeakPipelineTrigger.Register(ManualResetEvent watch)
        {
            onReady = watch;

            if (items.Count > 0)
                onReady.Set();
        }

        void LeakPipelineTrigger.Execute()
        {
            RepositoryTask task;
            onReady.Reset();

            if (stopped == false)
            {
                while (items.TryDequeue(out task))
                {
                    ready.Enqueue(task);
                }

                int count = ready.Count;

                while (count-- > 0)
                {
                    task = ready.Dequeue();

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
        }

        private void OnCompleted(RepositoryTask task)
        {
            task.Release(this);
            onReady.Set();
        }
    }
}