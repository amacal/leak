using Leak.Common;

namespace Leak.Events
{
    public class MetadataPieceRequested
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;
    }
}