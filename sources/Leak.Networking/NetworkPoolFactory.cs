using Leak.Completion;
using Leak.Tasks;

namespace Leak.Networking
{
    public class NetworkPoolFactory
    {
        private readonly NetworkPoolDependency dependency;

        public NetworkPoolFactory(LeakPipeline pipeline, CompletionWorker completion)
        {
            dependency = new NetworkPoolDependency
            {
                Pipeline = pipeline,
                Completion = completion
            };
        }

        public NetworkPool CreateInstance(NetworkPoolHooks hooks)
        {
            return new NetworkPoolInstance(dependency, hooks);
        }
    }
}