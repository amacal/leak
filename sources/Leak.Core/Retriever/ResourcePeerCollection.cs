using Leak.Core.Common;
using System.Collections;
using System.Collections.Generic;

namespace Leak.Core.Retriever
{
    public class ResourcePeerCollection : IEnumerable<ResourcePeer>
    {
        private readonly Dictionary<PeerHash, ResourcePeer> items;

        public ResourcePeerCollection()
        {
            this.items = new Dictionary<PeerHash, ResourcePeer>();
        }

        public void AddPeer(PeerHash peer)
        {
            items.Add(peer, new ResourcePeer(peer));
        }

        public void Choke(PeerHash peer)
        {
            items[peer].Choke();
        }

        public void Unchoke(PeerHash peer)
        {
            items[peer].Unchoke();
        }

        public void Increase(PeerHash peer)
        {
            items[peer].Increase();
        }

        public void Decrease(PeerHash peer)
        {
            items[peer].Decrease();
        }

        public IEnumerator<ResourcePeer> GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}