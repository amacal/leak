using Leak.Common;
using Leak.Extensions;

namespace Leak.Glue
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

        public GlueBuilder WithBlocks(DataBlockFactory blocks)
        {
            dependencies.Blocks = blocks;
            return this;
        }

        public GlueBuilder WithPlugin(MorePlugin plugin)
        {
            configuration.Plugins.Add(plugin);
            return this;
        }

        public GlueService Build()
        {
            return new GlueImplementation(parameters, dependencies, new GlueHooks(), configuration);
        }

        public GlueService Build(GlueHooks hooks)
        {
            return new GlueImplementation(parameters, dependencies, hooks, configuration);
        }
    }
}