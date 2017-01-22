using Leak.Common;
using Leak.Tasks;

namespace Leak.Metaget
{
    public class MetagetBuilder
    {
        private readonly MetagetParameters parameters;
        private readonly MetagetDependencies dependencies;
        private readonly MetagetHooks hooks;
        private readonly MetagetConfiguration configuration;

        public MetagetBuilder()
        {
            parameters = new MetagetParameters();
            dependencies = new MetagetDependencies();
            hooks = new MetagetHooks();
            configuration = new MetagetConfiguration();
        }

        public MetagetBuilder WithHash(FileHash hash)
        {
            parameters.Hash = hash;
            return this;
        }

        public MetagetBuilder WithPipeline(PipelineService pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public MetagetBuilder WithGlue(MetagetGlue glue)
        {
            dependencies.Glue = glue;
            return this;
        }

        public MetagetBuilder WithMetafile(MetagetMetafile metafile)
        {
            dependencies.Metafile = metafile;
            return this;
        }

        public MetagetService Build()
        {
            return new MetagetService(parameters, dependencies, hooks, configuration);
        }
    }
}