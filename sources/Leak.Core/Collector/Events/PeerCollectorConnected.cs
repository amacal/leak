using Leak.Core.Common;

namespace Leak.Core.Collector.Events
{
    public class PeerCollectorConnected
    {
        private readonly PeerAddress peer;
        private readonly int total;

        public PeerCollectorConnected(PeerAddress peer, int total)
        {
            this.peer = peer;
            this.total = total;
        }

        public PeerAddress Peer
        {
            get { return peer; }
        }

        public int Total
        {
            get { return total; }
        }
    }
}