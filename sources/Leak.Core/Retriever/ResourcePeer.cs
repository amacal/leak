using Leak.Core.Common;
using System;

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

        public void Increase(int value)
        {
            rank = Math.Min(8192, rank + value);
        }

        public void Decrease()
        {
            rank = Math.Max(0, rank - 1);
        }

        public void Decrease(int value)
        {
            rank = Math.Max(0, rank - value);
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