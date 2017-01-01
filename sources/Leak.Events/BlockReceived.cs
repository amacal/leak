using Leak.Common;

namespace Leak.Events
{
    public class BlockReceived
    {
        public FileHash Hash;

        public PeerHash Peer;

        public BlockIndex Block;

        public DataBlock Payload;
    }
}