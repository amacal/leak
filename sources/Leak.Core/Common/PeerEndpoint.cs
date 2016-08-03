namespace Leak.Core.Common
{
    public class PeerEndpoint
    {
        private readonly FileHash hash;
        private readonly PeerHash peer;
        private readonly PeerAddress remote;
        private readonly PeerDirection direction;

        public PeerEndpoint(FileHash hash, PeerHash peer, PeerAddress remote, PeerDirection direction)
        {
            this.hash = hash;
            this.peer = peer;
            this.remote = remote;
            this.direction = direction;
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public PeerHash Peer
        {
            get { return peer; }
        }

        public PeerAddress Remote
        {
            get { return remote; }
        }

        public PeerDirection Direction
        {
            get { return direction; }
        }
    }
}