using Leak.Core.Events;
using Leak.Core.Glue;
using Leak.Core.Glue.Extensions.Metadata;

namespace Leak.Core.Tests.Network
{
    public class SwarmEntry
    {
        public GlueService Glue;

        public GlueHooks Hooks;

        public MetadataHooks Metadata;

        public Trigger<PeerConnected> Connected;

        public Trigger<ExtensionListReceived> Exchanged;
    }
}