using Leak.Common;

namespace Leak.Events
{
    public class MetadataPieceReceived
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;
    }
}