using Leak.Completion;
using Leak.Tasks;

namespace Leak.Networking.Tests
{
    public class NetworkFixture
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread worker;
        private readonly NetworkPool pool;
        private readonly NetworkPoolHooks hooks;

        public NetworkFixture()
        {
            pipeline = new LeakPipeline();
            worker = new CompletionThread();

            hooks = new NetworkPoolHooks();
            pool = new NetworkPoolFactory(pipeline, worker).CreateInstance(hooks);
        }

        public NetworkPool Pool
        {
            get { return pool; }
        }

        public NetworkPoolHooks Hooks
        {
            get { return hooks; }
        }

        public void Start()
        {
            worker.Start();
            pipeline.Start();
            pool.Start();
        }

        public void Stop()
        {
            worker.Dispose();
            pipeline.Stop();
        }
    }
}