using Leak.Common;
using Leak.Tasks;

namespace Leak.Data.Map
{
    public class OmnibusBuilder
    {
        private readonly OmnibusParameters parameters;
        private readonly OmnibusDependencies dependencies;
        private readonly OmnibusConfiguration configuration;

        public OmnibusBuilder()
        {
            parameters = new OmnibusParameters();
            dependencies = new OmnibusDependencies();
            configuration = new OmnibusConfiguration();
        }

        public OmnibusBuilder WithHash(FileHash hash)
        {
            parameters.Hash = hash;
            return this;
        }

        public OmnibusBuilder WithPipeline(PipelineService pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public OmnibusBuilder WithSchedulerThreshold(int threshold)
        {
            configuration.SchedulerThreshold = threshold;
            return this;
        }

        public OmnibusBuilder WithPoolSize(int poolSize)
        {
            configuration.PoolSize = poolSize;
            return this;
        }

        public OmnibusService Build()
        {
            return Build(new OmnibusHooks());
        }

        public OmnibusService Build(OmnibusHooks hooks)
        {
            return new OmnibusService(parameters, dependencies, configuration, hooks);
        }
    }
}