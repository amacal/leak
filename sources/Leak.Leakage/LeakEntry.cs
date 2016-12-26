using Leak.Common;
using Leak.Connector;
using Leak.Extensions.Metadata;
using Leak.Extensions.Peers;
using Leak.Glue;
using Leak.Negotiator;
using Leak.Spartan;

namespace Leak.Leakage
{
    public class LeakEntry
    {
        public FileHash Hash;
        public string Destination;

        public MetadataPlugin MetadataPlugin;
        public MetadataHooks MetadataHooks;

        public PeersPlugin PeersPlugin;
        public PeersHooks PeersHooks;

        public HandshakeNegotiator Negotiator;
        public HandshakeNegotiatorHooks NegotiatorHooks;

        public GlueService Glue;
        public GlueHooks GlueHooks;

        public SpartanService Spartan;
        public SpartanHooks SpartaHooks;

        public PeerConnector Connector;
        public PeerConnectorHooks ConnectorHooks;
    }
}