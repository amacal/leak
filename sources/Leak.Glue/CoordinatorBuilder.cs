using Leak.Common;
using Leak.Extensions;
using Leak.Networking.Core;
using Leak.Tasks;

namespace Leak.Peer.Coordinator
{
    public class CoordinatorBuilder
    {
        private readonly CoordinatorParameters parameters;
        private readonly CoordinatorDependencies dependencies;
        private readonly CoordinatorConfiguration configuration;

        public CoordinatorBuilder()
        {
            parameters = new CoordinatorParameters();
            dependencies = new CoordinatorDependencies();
            configuration = new CoordinatorConfiguration();
        }

        public CoordinatorBuilder WithHash(FileHash hash)
        {
            parameters.Hash = hash;
            return this;
        }

        public CoordinatorBuilder WithMemory(DataBlockFactory blocks)
        {
            dependencies.Blocks = blocks;
            return this;
        }

        public CoordinatorBuilder WithPipeline(PipelineService pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public CoordinatorBuilder WithPlugin(MorePlugin plugin)
        {
            configuration.Plugins.Add(plugin);
            return this;
        }

        public CoordinatorService Build()
        {
            return Build(new CoordinatorHooks());
        }

        public CoordinatorService Build(CoordinatorHooks hooks)
        {
            return new CoordinatorService(parameters, dependencies, hooks, configuration);
        }
    }
}