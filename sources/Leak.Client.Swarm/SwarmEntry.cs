using System.Collections.Generic;
using Leak.Common;
using Leak.Connector;
using Leak.Glue;
using Leak.Negotiator;

namespace Leak.Client.Swarm
{
    public class SwarmEntry
    {
        public HashSet<PeerHash> Peers { get; set; }
        public PeerConnector Connector { get; set; }
        public HandshakeNegotiator Negotiator { get; set; }
        public GlueService Glue { get; set; }
    }
}