using Leak.Common;
using Leak.Networking.Core;

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