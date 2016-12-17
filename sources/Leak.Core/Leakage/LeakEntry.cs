using Leak.Common;
using Leak.Connector;
using Leak.Core.Spartan;
using Leak.Extensions.Metadata;
using Leak.Glue;

namespace Leak.Core.Leakage
{
    public class LeakEntry
    {
        public FileHash Hash;
        public string Destination;

        public MetadataHooks MetadataHooks;

        public GlueService Glue;
        public GlueHooks GlueHooks;

        public SpartanService Spartan;
        public SpartanHooks SpartaHooks;

        public PeerConnector Connector;
        public PeerConnectorHooks ConnectorHooks;
    }
}