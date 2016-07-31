using Leak.Core.Common;
using Leak.Core.Network;

namespace Leak.Core.Connector
{
    public class PeerConnectorConfiguration
    {
        public PeerHash Peer { get; set; }

        public FileHash Hash { get; set; }

        public PeerConnectorCallback Callback { get; set; }

        public NetworkPool Pool { get; set; }

        public bool Extensions { get; set; }
    }
}