using Leak.Common;
using Leak.Files;
using Leak.Tasks;

namespace Leak.Meta.Store
{
    public class MetafileBuilder
    {
        private readonly MetafileParameters parameters;
        private readonly MetafileDependencies dependencies;

        public MetafileBuilder()
        {
            parameters = new MetafileParameters();
            dependencies = new MetafileDependencies();
        }

        public MetafileBuilder WithHash(FileHash hash)
        {
            parameters.Hash = hash;
            return this;
        }

        public MetafileBuilder WithDestination(string destination)
        {
            parameters.Destination = destination;
            return this;
        }

        public MetafileBuilder WithFiles(FileFactory files)
        {
            dependencies.Files = files;
            return this;
        }

        public MetafileBuilder WithPipeline(PipelineService pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public MetafileService Build()
        {
            return Build(new MetafileHooks());
        }

        public MetafileService Build(MetafileHooks hooks)
        {
            return new MetafileImplementation(parameters, dependencies, hooks);
        }
    }
}