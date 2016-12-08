using Leak.Common;
using Leak.Core.Glue;
using Leak.Core.Glue.Extensions.Metadata;
using Leak.Events;

namespace Leak.Core.Tests.Core
{
    public class SwarmEntry
    {
        public int? Port;

        public PeerHash Peer;

        public GlueService Glue;

        public GlueHooks Hooks;

        public MetadataHooks Metadata;

        public Trigger<PeerConnected> Connected;

        public Trigger<ExtensionListReceived> Exchanged;
    }
}