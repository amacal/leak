using Leak.Common;

namespace Leak.Core.Events
{
    public class BlockHandled
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;

        public int Block;

        public int Size;
    }
}