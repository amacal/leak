using Leak.Completion;
using Leak.Tasks;

namespace Leak.Networking
{
    public class NetworkPoolDependencies
    {
        public LeakPipeline Pipeline;

        public CompletionWorker Completion;
    }
}