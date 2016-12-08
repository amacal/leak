using Leak.Common;

namespace Leak.Events
{
    public class BlockReceived
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;

        public int Block;

        public DataBlock Payload;
    }
}