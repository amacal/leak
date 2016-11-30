using Leak.Core.Common;

namespace Leak.Core.Events
{
    public class BlockRequested
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;

        public int Block;
    }
}