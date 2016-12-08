using Leak.Common;

namespace Leak.Events
{
    public class BlockRequested
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;

        public int Block;
    }
}