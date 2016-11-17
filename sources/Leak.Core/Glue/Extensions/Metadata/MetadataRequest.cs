using Leak.Core.Common;

namespace Leak.Core.Glue.Extensions.Metadata
{
    public class MetadataRequest
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;
    }
}