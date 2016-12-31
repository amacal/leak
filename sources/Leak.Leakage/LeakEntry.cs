using Leak.Common;
using Leak.Connector;
using Leak.Extensions.Metadata;
using Leak.Extensions.Peers;
using Leak.Glue;
using Leak.Metafile;
using Leak.Metaget;
using Leak.Negotiator;
using Leak.Spartan;

namespace Leak.Leakage
{
    public class LeakEntry
    {
        public FileHash Hash;
        public string Destination;

        public MetadataPlugin MetadataPlugin;
        public PeersPlugin PeersPlugin;
        public GlueService Glue;

        public MetafileService Metafile;
        public MetagetService Metaget;
        public SpartanService Spartan;

        public PeerConnector Connector;
        public HandshakeNegotiator Negotiator;
    }
}