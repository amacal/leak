using Leak.Core.Common;

namespace Leak.Core.Events
{
    public class BlockReserved
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;

        public int Block;

        public int Size;
    }
}