using Leak.Common;

namespace Leak.Peer.Coordinator.Core
{
    public class Request
    {
        private readonly BlockIndex block;

        public Request(BlockIndex block)
        {
            this.block = block;
        }

        public BlockIndex Block
        {
            get { return block; }
        }
    }
}