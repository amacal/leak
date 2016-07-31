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

        public bool AddPeer(PeerHash peer)
        {
            ResourcePeer entry;
            items.TryGetValue(peer, out entry);

            if (entry == null)
            {
                items.Add(peer, new ResourcePeer(peer));
            }

            return entry == null;
        }

        public void Choke(PeerHash peer)
        {
            items[peer].Choke();
        }

        public void Unchoke(PeerHash peer)
        {
            items[peer].Unchoke();
        }

        public bool IsExtended(PeerHash peer)
        {
            return items[peer].IsExtended();
        }

        public void Extend(PeerHash peer)
        {
            items[peer].Extend();
        }

        public void Increase(PeerHash peer)
        {
            items[peer].Increase(2);
        }

        public void Decrease(PeerHash peer)
        {
            items[peer].Decrease();
        }

        public void Decrease(PeerHash peer, int value)
        {
            ResourcePeer entry;

            if (peer != null && items.TryGetValue(peer, out entry))
            {
                entry.Decrease(value);
            }
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