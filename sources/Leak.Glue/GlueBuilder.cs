using Leak.Common;
using Leak.Extensions;
using Leak.Networking.Core;
using Leak.Peer.Receiver;
using Leak.Tasks;

namespace Leak.Peer.Coordinator
{
    public class GlueBuilder
    {
        private readonly GlueParameters parameters;
        private readonly GlueDependencies dependencies;
        private readonly GlueConfiguration configuration;

        public GlueBuilder()
        {
            parameters = new GlueParameters();
            dependencies = new GlueDependencies();
            configuration = new GlueConfiguration();
        }

        public GlueBuilder WithHash(FileHash hash)
        {
            parameters.Hash = hash;
            return this;
        }

        public GlueBuilder WithMemory(DataBlockFactory blocks)
        {
            dependencies.Blocks = blocks;
            return this;
        }

        public GlueBuilder WithPipeline(PipelineService pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public GlueBuilder WithPlugin(MorePlugin plugin)
        {
            configuration.Plugins.Add(plugin);
            return this;
        }

        public GlueBuilder WithDefinition(ReceiverDefinition definition)
        {
            configuration.Definition = definition;
            return this;
        }

        public GlueService Build()
        {
            return Build(new GlueHooks());
        }

        public GlueService Build(GlueHooks hooks)
        {
            return new GlueImplementation(parameters, dependencies, hooks, configuration);
        }
    }
}