namespace Leak.Core.Common
{
    public class PeerEndpoint
    {
        private readonly FileHash hash;
        private readonly PeerHash peer;
        private readonly string remote;
        private readonly PeerDirection direction;

        public PeerEndpoint(FileHash hash, PeerHash peer, string remote, PeerDirection direction)
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

        public string Remote
        {
            get { return remote; }
        }

        public PeerDirection Direction
        {
            get { return direction; }
        }
    }
}