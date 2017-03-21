using Leak.Completion;
using Leak.Tasks;
using System;

namespace Leak.Networking.Tests
{
    public class NetworkFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread worker;
        private readonly NetworkPool pool;
        private readonly NetworkPoolHooks hooks;

        public NetworkFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();

            worker = new CompletionThread();
            worker.Start();

            hooks = new NetworkPoolHooks();

            pool =
                new NetworkPoolBuilder()
                    .WithPipeline(pipeline)
                    .WithWorker(worker)
                    .WithMemory(new NetworkMemory())
                    .Build(hooks);

            pool.Start();
        }

        public NetworkPool Pool
        {
            get { return pool; }
        }

        public NetworkPoolHooks Hooks
        {
            get { return hooks; }
        }

        public void Dispose()
        {
            worker?.Dispose();
            pipeline?.Stop();
        }
    }
}