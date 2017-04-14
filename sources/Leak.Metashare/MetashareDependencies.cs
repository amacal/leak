using Leak.Meta.Store;
using Leak.Peer.Coordinator;
using Leak.Tasks;

namespace Leak.Meta.Share
{
    public class MetashareDependencies
    {
        public PipelineService Pipeline;

        public CoordinatorService Glue;

        public MetafileService Metafile;
    }
}