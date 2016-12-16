using Leak.Common;

namespace Leak.Connector
{
    public class PeerConnectorConfiguration
    {
        public PeerConnectorConfiguration()
        {
            Peer = PeerHash.Random();
            Extensions = true;
        }

        public PeerHash Peer { get; set; }

        public bool Extensions { get; set; }
    }
}