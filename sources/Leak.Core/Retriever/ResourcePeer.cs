using Leak.Core.Common;

namespace Leak.Core.Retriever
{
    public class ResourcePeer
    {
        private readonly PeerHash hash;
        private ResourcePeerStatus status;
        private int rank;

        public ResourcePeer(PeerHash hash)
        {
            this.hash = hash;
            this.status = ResourcePeerStatus.None;
        }

        public PeerHash Hash
        {
            get { return hash; }
        }

        public int Rank
        {
            get { return rank; }
        }

        public void Increase()
        {
            rank++;
        }

        public void Decrease()
        {
            rank--;
        }

        public bool IsUnchoke()
        {
            return status.HasFlag(ResourcePeerStatus.Unchoke);
        }

        public void Unchoke()
        {
            status = status | ResourcePeerStatus.Unchoke;
        }

        public void Choke()
        {
            status = status ^ ResourcePeerStatus.Unchoke;
        }
    }
}