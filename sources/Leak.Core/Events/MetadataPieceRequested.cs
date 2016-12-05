using Leak.Common;

namespace Leak.Core.Events
{
    public class MetadataPieceRequested
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;
    }
}