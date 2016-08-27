using Leak.Core.Common;
using Leak.Core.Listener;

namespace Leak.Core.Collector
{
    public class PeerCollectorListenerStarted
    {
        private readonly PeerListenerStarted inner;

        public PeerCollectorListenerStarted(PeerListenerStarted inner)
        {
            this.inner = inner;
        }

        public PeerHash Peer
        {
            get { return inner.Hash; }
        }

        public int Port
        {
            get { return inner.Port; }
        }
    }
}