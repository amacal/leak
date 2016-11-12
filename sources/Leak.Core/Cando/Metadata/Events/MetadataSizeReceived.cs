using Leak.Core.Common;

namespace Leak.Core.Cando.Metadata.Events
{
    public class MetadataSizeReceived
    {
        public FileHash Hash;

        public PeerHash Peer;

        public MetadataSize Size;
    }
}