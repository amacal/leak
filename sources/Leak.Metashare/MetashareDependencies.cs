using Leak.Glue;
using Leak.Meta.Store;
using Leak.Tasks;

namespace Leak.Meta.Share
{
    public class MetashareDependencies
    {
        public LeakPipeline Pipeline;

        public GlueService Glue;

        public MetafileService Metafile;
    }
}