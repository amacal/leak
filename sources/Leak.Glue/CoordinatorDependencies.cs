using Leak.Networking.Core;
using Leak.Tasks;

namespace Leak.Peer.Coordinator
{
    public class CoordinatorDependencies
    {
        public DataBlockFactory Blocks;

        public PipelineService Pipeline;
    }
}