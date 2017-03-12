using Leak.Common;
using Leak.Tasks;

namespace Leak.Data.Get
{
    public class RetrieverBuilder
    {
        private readonly RetrieverParameters parameters;
        private readonly RetrieverDependencies dependencies;
        private readonly RetrieverConfiguration configuration;

        public RetrieverBuilder()
        {
            parameters = new RetrieverParameters();
            dependencies = new RetrieverDependencies();
            configuration = new RetrieverConfiguration();
        }

        public RetrieverBuilder WithHash(FileHash hash)
        {
            parameters.Hash = hash;
            return this;
        }

        public RetrieverBuilder WithPipeline(PipelineService pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public RetrieverBuilder WithGlue(RetrieverGlue glue)
        {
            dependencies.Glue = glue;
            return this;
        }

        public RetrieverBuilder WithOmnibus(RetrieverOmnibus omnibus)
        {
            dependencies.Omnibus = omnibus;
            return this;
        }

        public RetrieverBuilder WithRepository(RetrieverRepository repository)
        {
            dependencies.Repository = repository;
            return this;
        }

        public RetrieverService Build()
        {
            return Build(new RetrieverHooks());
        }

        public RetrieverService Build(RetrieverHooks hooks)
        {
            return new RetrieverService(parameters, dependencies, configuration, hooks);
        }
    }
}