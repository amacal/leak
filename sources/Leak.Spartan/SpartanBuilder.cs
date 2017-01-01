using Leak.Common;
using Leak.Files;
using Leak.Glue;
using Leak.Metaget;
using Leak.Metashare;
using Leak.Retriever;
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

        public SpartanBuilder WithDestination(string destination)
        {
            parameters.Destination = destination;
            return this;
        }

        public SpartanBuilder WithPipeline(LeakPipeline pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public SpartanBuilder WithFiles(FileFactory files)
        {
            dependencies.Files = files;
            return this;
        }

        public SpartanBuilder WithGlue(GlueService glue)
        {
            dependencies.Glue = glue;
            return this;
        }

        public SpartanBuilder WithMetaget(MetagetService metaget)
        {
            dependencies.Metaget = metaget;
            return this;
        }

        public SpartanBuilder WithMetashare(MetashareService metashare)
        {
            dependencies.Metashare = metashare;
            return this;
        }

        public SpartanBuilder WithRetriever(RetrieverService retriever)
        {
            dependencies.Retriever = retriever;
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
