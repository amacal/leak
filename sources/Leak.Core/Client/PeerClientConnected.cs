using Leak.Core.Collector.Events;
using Leak.Core.Common;

namespace Leak.Core.Client
{
    public class PeerClientConnected
    {
        private readonly PeerCollectorConnected inner;

        public PeerClientConnected(PeerCollectorConnected inner)
        {
            this.inner = inner;
        }

        public PeerAddress Peer
        {
            get { return inner.Peer; }
        }

        public int Total
        {
            get { return inner.Total; }
        }
    }
}