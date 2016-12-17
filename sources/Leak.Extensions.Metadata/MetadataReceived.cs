using Leak.Common;

namespace Leak.Extensions.Metadata
{
    public class MetadataReceived
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;

        public byte[] Data;
    }
}