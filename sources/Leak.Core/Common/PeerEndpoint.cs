namespace Leak.Core.Common
{
    public class PeerEndpoint
    {
        private readonly FileHash hash;
        private readonly PeerHash peer;
        private readonly string remote;

        public PeerEndpoint(FileHash hash, PeerHash peer, string remote)
        {
            this.hash = hash;
            this.peer = peer;
            this.remote = remote;
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
    }
}