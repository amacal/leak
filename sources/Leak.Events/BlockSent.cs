using Leak.Common;

namespace Leak.Events
{
    public class BlockSent
    {
        public FileHash Hash;

        public PeerHash Peer;

        public BlockIndex Block;
    }
}