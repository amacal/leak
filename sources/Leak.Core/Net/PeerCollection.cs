using System;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Net
{
    public class PeerCollection
    {
        private readonly List<Peer> items;

        public PeerCollection()
        {
            this.items = new List<Peer>();
        }

        public void Add(Peer peer)
        {
            items.Add(peer);
        }

        public bool Contains(TrackerResponsePeer peer)
        {
            return items.Any(x => x.Channel.Name == peer.Host);
        }

        public Peer ByChannel(PeerChannel channel)
        {
            return items.First(x => x.Channel == channel);
        }

        public void ForEach(Action<Peer> callback)
        {
            foreach (Peer peer in items)
            {
                callback.Invoke(peer);
            }
        }
    }
}