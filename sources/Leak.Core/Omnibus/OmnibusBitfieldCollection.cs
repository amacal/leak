using Leak.Core.Common;
using Leak.Core.Messages;
using System.Collections.Generic;

namespace Leak.Core.Omnibus
{
    public class OmnibusBitfieldCollection
    {
        private readonly Dictionary<PeerHash, Bitfield> byPeer;

        public OmnibusBitfieldCollection()
        {
            this.byPeer = new Dictionary<PeerHash, Bitfield>();
        }

        public void Add(PeerHash peer, Bitfield bitfield)
        {
            byPeer[peer] = bitfield;
        }

        public bool Contains(PeerHash peer)
        {
            return byPeer.ContainsKey(peer);
        }

        public bool IsComplete(PeerHash peer)
        {
            Bitfield bitfield;
            byPeer.TryGetValue(peer, out bitfield);

            return bitfield?.IsCompleted() == true;
        }

        public Bitfield ByPeer(PeerHash peer)
        {
            Bitfield bitfield;
            byPeer.TryGetValue(peer, out bitfield);
            return bitfield;
        }
    }
}