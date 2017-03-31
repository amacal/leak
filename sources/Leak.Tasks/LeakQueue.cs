using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Leak.Tasks
{
    public class LeakQueue<TContext> : LeakPipelineTrigger
    {
        private readonly TContext context;
        private readonly Queue<LeakTask<TContext>> ready;
        private readonly ConcurrentQueue<LeakTask<TContext>> items;

        private ManualResetEvent onReady;
        private bool completed;

        public LeakQueue(TContext context)
        {
            this.context = context;

            ready = new Queue<LeakTask<TContext>>();
            items = new ConcurrentQueue<LeakTask<TContext>>();
            onReady = new ManualResetEvent(false);
        }

        public void Add(LeakTask<TContext> task)
        {
            if (completed == false)
            {
                items.Enqueue(task);
                onReady.Set();
            }
        }

        public void Stop()
        {
            LeakTask<TContext> task;
            completed = true;

            while (items.TryDequeue(out task))
            {
            }
        }

        void LeakPipelineTrigger.Register(ManualResetEvent watch)
        {
            onReady = watch;
        }

        void LeakPipelineTrigger.Execute()
        {
            LeakTask<TContext> task;

            if (completed == false)
            {
                onReady.Reset();

                while (items.TryDequeue(out task))
                {
                    ready.Enqueue(task);
                }

                while (ready.Count > 0)
                {
                    task = ready.Dequeue();
                    task.Execute(context);
                }
            }
        }
    }
}