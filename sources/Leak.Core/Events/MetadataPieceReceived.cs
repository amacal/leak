using Leak.Core.Common;

namespace Leak.Core.Events
{
    public class MetadataPieceReceived
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;
    }
}