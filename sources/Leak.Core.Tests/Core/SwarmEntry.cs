using Leak.Common;
using Leak.Events;
using Leak.Glue;
using Leak.Glue.Extensions.Metadata;
using Leak.Testing;

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