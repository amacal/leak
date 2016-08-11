using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Listener;

namespace Leak.Core.Collector
{
    public static class PeerCollectorExtensions
    {
        public static PeerEndpoint ToEndpoint(this PeerListenerHandshake handshake, PeerAddress address)
        {
            return new PeerEndpoint(handshake.Hash, handshake.Peer, address, PeerDirection.Incoming);
        }

        public static PeerEndpoint ToEndpoint(this PeerConnectorHandshake handshake, PeerAddress address)
        {
            return new PeerEndpoint(handshake.Hash, handshake.Peer, address, PeerDirection.Outgoing);
        }
    }
}