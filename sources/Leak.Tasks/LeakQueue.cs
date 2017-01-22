using System.Collections.Concurrent;
using System.Threading;

namespace Leak.Tasks
{
    public class LeakQueue<TContext> : LeakPipelineTrigger
    {
        private readonly TContext context;
        private readonly ConcurrentQueue<LeakTask<TContext>> items;

        private ManualResetEvent onReady;

        public LeakQueue(TContext context)
        {
            this.context = context;

            items = new ConcurrentQueue<LeakTask<TContext>>();
            onReady = new ManualResetEvent(false);
        }

        public void Add(LeakTask<TContext> task)
        {
            items.Enqueue(task);
            onReady.Set();
        }

        public void Clear()
        {
            LeakTask<TContext> task;

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

            while (items.TryDequeue(out task))
            {
                onReady.Reset();
                task.Execute(context);
            }
        }
    }
}