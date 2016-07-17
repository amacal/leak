using Leak.Core.Common;
using Leak.Core.Loop;
using Leak.Core.Network;

namespace Leak.Core.Collector
{
    public class PeerCollectorStorageEntry
    {
        public FileHash Hash { get; set; }

        public PeerHash Peer { get; set; }

        public NetworkConnection Connection { get; set; }

        public ConnectionLoopChannel Channel { get; set; }
    }
}