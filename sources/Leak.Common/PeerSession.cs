namespace Leak.Common
{
    public class PeerSession
    {
        private readonly FileHash hash;
        private readonly PeerHash peer;

        public PeerSession(FileHash hash, PeerHash peer)
        {
            this.hash = hash;
            this.peer = peer;
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public PeerHash Peer
        {
            get { return peer; }
        }

        public override int GetHashCode()
        {
            return peer.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            PeerSession other = obj as PeerSession;

            return other != null &&
                   other.Peer.Equals(Peer) &&
                   other.Hash.Equals(Hash);
        }
    }
}