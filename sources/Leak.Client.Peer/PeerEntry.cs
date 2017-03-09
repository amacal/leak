using Leak.Common;
using Leak.Connector;
using Leak.Glue;
using Leak.Negotiator;

namespace Leak.Client.Peer
{
    public class PeerEntry
    {
        public PeerHash Peer { get; set; }
        public PeerConnector Connector { get; set; }
        public HandshakeNegotiator Negotiator { get; set; }
        public GlueService Glue { get; set; }
        public byte[] Metadata { get; set; }
    }
}