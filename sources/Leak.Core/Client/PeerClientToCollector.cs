using Leak.Core.Collector;
using Leak.Core.Common;
using Leak.Core.Messages;

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

        public override void OnConnected(PeerHash peer, FileHash hash)
        {
            storage.AddPeer(hash, peer);
        }

        public override void OnBitfield(PeerHash peer, BitfieldMessage message)
        {
            storage.AddBitfield(peer, new Bitfield());
        }
    }
}