using Leak.Common;

namespace Leak.Peer.Coordinator.Events
{
    public class BlockRequested
    {
        public FileHash Hash;

        public PeerHash Peer;

        public BlockIndex Block;
    }
}