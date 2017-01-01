using Leak.Common;

namespace Leak.Events
{
    public class BlockHandled
    {
        public FileHash Hash;

        public PeerHash Peer;

        public BlockIndex Block;
    }
}