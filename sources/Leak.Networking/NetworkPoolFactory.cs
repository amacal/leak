using Leak.Completion;
using Leak.Tasks;

namespace Leak.Networking
{
    public class NetworkPoolFactory
    {
        private readonly NetworkPoolDependencies dependencies;

        public NetworkPoolFactory(LeakPipeline pipeline, CompletionWorker completion)
        {
            dependencies = new NetworkPoolDependencies
            {
                Pipeline = pipeline,
                Completion = completion
            };
        }

        public NetworkPool CreateInstance(NetworkPoolHooks hooks)
        {
            return new NetworkPoolInstance(dependencies, hooks);
        }
    }
}