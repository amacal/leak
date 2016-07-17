using Leak.Core.Common;

namespace Leak.Core.Connector
{
    public class PeerConnectorConfiguration
    {
        public PeerHash Peer { get; set; }

        public FileHash Hash { get; set; }

        public PeerConnectorCallback Callback { get; set; }
    }
}