using Leak.Common;

namespace Leak.Core.Glue.Extensions.Metadata
{
    public class MetadataReceived
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;

        public byte[] Data;
    }
}