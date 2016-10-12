using System.Threading;

namespace Leak.Core.Core
{
    public abstract class LeakQueueBase<TContext>
    {
        private readonly Thread worker;
        protected readonly ManualResetEventSlim onReady;

        protected LeakQueueBase()
        {
            worker = new Thread(Process);
            onReady = new ManualResetEventSlim(false);
        }

        public void Start(TContext context)
        {
            worker.Start(context);
            onReady.Set();
        }

        private void Process(object state)
        {
            TContext context = (TContext)state;

            while (true)
            {
                if (onReady.Wait(1000))
                {
                    onReady.Reset();
                    OnProcess(context);
                }
            }
        }

        protected abstract void OnProcess(TContext context);
    }
}