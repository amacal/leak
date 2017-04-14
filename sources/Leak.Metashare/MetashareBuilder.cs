using Leak.Common;
using Leak.Meta.Store;
using Leak.Peer.Coordinator;
using Leak.Tasks;

namespace Leak.Meta.Share
{
    public class MetashareBuilder
    {
        private readonly MetashareParameters parameters;
        private readonly MetashareDependencies dependencies;
        private readonly MetashareConfiguration configuration;

        public MetashareBuilder()
        {
            parameters = new MetashareParameters();
            dependencies = new MetashareDependencies();
            configuration = new MetashareConfiguration();
        }

        public MetashareBuilder WithHash(FileHash hash)
        {
            parameters.Hash = hash;
            return this;
        }

        public MetashareBuilder WithPipeline(PipelineService pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public MetashareBuilder WithGlue(CoordinatorService glue)
        {
            dependencies.Glue = glue;
            return this;
        }

        public MetashareBuilder WithMetafile(MetafileService metafile)
        {
            dependencies.Metafile = metafile;
            return this;
        }

        public MetashareService Build()
        {
            return Build(new MetashareHooks());
        }

        public MetashareService Build(MetashareHooks hooks)
        {
            return new MetashareService(parameters, dependencies, configuration, hooks);
        }
    }
}