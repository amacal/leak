using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Leak.Tasks;

namespace Leak.Networking
{
    public class NetworkPoolQueue : LeakPipelineTrigger
    {
        private readonly NetworkPoolInstance context;
        private readonly Queue<NetworkPoolTask> ready;
        private readonly ConcurrentQueue<NetworkPoolTask> items;
        private readonly HashSet<object> keys;

        private ManualResetEvent onReady;
        private bool stopped;

        public NetworkPoolQueue(NetworkPoolInstance context)
        {
            this.context = context;

            ready = new Queue<NetworkPoolTask>();
            items = new ConcurrentQueue<NetworkPoolTask>();
            keys = new HashSet<object>();
        }

        public void Stop()
        {
            stopped = true;
        }

        public void Add(NetworkPoolTask task)
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
            NetworkPoolTask task;
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

        private void OnCompleted(NetworkPoolTask task)
        {
            task.Release(this);
            onReady.Set();
        }
    }
}