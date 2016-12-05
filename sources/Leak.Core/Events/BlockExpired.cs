using Leak.Common;

namespace Leak.Core.Events
{
    public class BlockExpired
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;

        public int Block;
    }
}