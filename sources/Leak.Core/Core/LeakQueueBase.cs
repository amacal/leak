using System.Threading;

namespace Leak.Core.Core
{
    public abstract class LeakQueueBase<TContext>
    {
        private readonly Thread worker;
        protected readonly ManualResetEventSlim onReady;

        private TContext data;

        protected LeakQueueBase()
        {
            onReady = new ManualResetEventSlim(false);
            worker = new Thread(Process);
            worker.Start();
        }

        public void Start(TContext context)
        {
            data = context;
            onReady.Set();
        }

        private void Process()
        {
            while (true)
            {
                if (onReady.Wait(1000))
                {
                    onReady.Reset();
                    OnProcess(data);
                }
            }
        }

        protected abstract void OnProcess(TContext context);
    }
}