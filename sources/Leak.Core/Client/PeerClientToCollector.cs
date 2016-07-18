using Leak.Core.Collector;
using Leak.Core.Common;

namespace Leak.Core.Client
{
    public class PeerClientToCollector : PeerCollectorCallbackBase
    {
        private readonly PeerClientConfiguration configuration;
        private readonly PeerClientStorage storage;

        public PeerClientToCollector(PeerClientConfiguration configuration, PeerClientStorage storage)
        {
            this.configuration = configuration;
            this.storage = storage;
        }

        public override void OnConnected(PeerHash hash)
        {
            base.OnConnected(hash);
        }
    }
}