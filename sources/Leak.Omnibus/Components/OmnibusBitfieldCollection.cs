using System.Collections.Generic;
using System.Linq;
using Leak.Common;

namespace Leak.Datamap.Components
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

        public Bitfield ByPeer(PeerHash peer)
        {
            Bitfield bitfield;
            byPeer.TryGetValue(peer, out bitfield);
            return bitfield;
        }

        public Bitfield[] ToArray()
        {
            return byPeer.Values.ToArray();
        }
    }
}