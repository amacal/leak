using Leak.Common;
using Leak.Core.Connector;
using Leak.Core.Glue;
using Leak.Core.Glue.Extensions.Metadata;
using Leak.Core.Spartan;

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