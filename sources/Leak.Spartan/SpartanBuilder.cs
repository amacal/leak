using Leak.Common;
using Leak.Tasks;

namespace Leak.Spartan
{
    public class SpartanBuilder
    {
        private readonly SpartanParameters parameters;
        private readonly SpartanDependencies dependencies;
        private readonly SpartanHooks hooks;
        private readonly SpartanConfiguration configuration;

        public SpartanBuilder()
        {
            parameters = new SpartanParameters();
            dependencies = new SpartanDependencies();
            hooks = new SpartanHooks();
            configuration = new SpartanConfiguration();
        }

        public SpartanBuilder WithHash(FileHash hash)
        {
            parameters.Hash = hash;
            return this;
        }

        public SpartanBuilder WithPipeline(PipelineService pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public SpartanBuilder WithMetaget(SpartanMetaget metaget)
        {
            dependencies.Metaget = metaget;
            return this;
        }

        public SpartanBuilder WithMetashare(SpartanMetashare metashare)
        {
            dependencies.Metashare = metashare;
            return this;
        }

        public SpartanBuilder WithDatastore(SpartanRepository repository)
        {
            dependencies.Repository = repository;
            return this;
        }

        public SpartanBuilder WithDataget(SpartanRetriever retriever)
        {
            dependencies.Retriever = retriever;
            return this;
        }

        public SpartanBuilder WithDatashare(SpartanDatashare datashare)
        {
            dependencies.Datashare = datashare;
            return this;
        }

        public SpartanBuilder WithGoal(Goal goal)
        {
            configuration.Goal = goal;
            return this;
        }

        public SpartanService Build()
        {
            return new SpartanService(parameters, dependencies, hooks, configuration);
        }
    }
}