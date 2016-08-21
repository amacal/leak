using Leak.Core.Common;

namespace Leak.Core.Collector
{
    public class PeerCollectorConnected
    {
        private readonly FileHash hash;
        private readonly PeerAddress peer;
        private readonly int total;

        public PeerCollectorConnected(FileHash hash, PeerAddress peer, int total)
        {
            this.hash = hash;
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

        public FileHash Hash
        {
            get { return hash; }
        }
    }
}