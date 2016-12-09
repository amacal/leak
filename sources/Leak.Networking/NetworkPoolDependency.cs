using Leak.Completion;
using Leak.Tasks;

namespace Leak.Networking
{
    public class NetworkPoolDependency
    {
        public LeakPipeline Pipeline;

        public CompletionWorker Completion;
    }
}