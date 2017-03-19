using Leak.Completion;
using Leak.Tasks;

namespace Leak.Networking
{
    public class NetworkPoolBuilder
    {
        private readonly NetworkPoolConfiguration configuration;
        private readonly NetworkPoolDependencies dependencies;

        public NetworkPoolBuilder()
        {
            configuration = new NetworkPoolConfiguration();
            dependencies = new NetworkPoolDependencies();
        }

        public NetworkPool Build()
        {
            return Build(new NetworkPoolHooks());
        }

        public NetworkPoolBuilder WithPipeline(PipelineService pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public NetworkPoolBuilder WithWorker(CompletionWorker worker)
        {
            dependencies.Completion = worker;
            return this;
        }

        public NetworkPoolBuilder WithMemory(NetworkPoolMemory poolMemory)
        {
            dependencies.Memory = poolMemory;
            return this;
        }

        public NetworkPoolBuilder WithBufferSize(int bufferSize)
        {
            configuration.BufferSize = bufferSize;
            return this;
        }

        public NetworkPool Build(NetworkPoolHooks hooks)
        {
            return new NetworkPoolInstance(dependencies, configuration, hooks);
        }
    }
}