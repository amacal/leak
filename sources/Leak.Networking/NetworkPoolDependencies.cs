using Leak.Completion;
using Leak.Tasks;

namespace Leak.Networking
{
    public class NetworkPoolDependencies
    {
        public PipelineService Pipeline;

        public CompletionWorker Completion;

        public NetworkPoolMemory Memory;
    }
}