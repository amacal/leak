using Leak.Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Omnibus
{
    public class OmnibusBitfieldCollection
    {
        private readonly Dictionary<PeerHash, Bitfield> byPeer;

        public OmnibusBitfieldCollection()
        {
            byPeer = new Dictionary<PeerHash, Bitfield>();
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

        public Bitfield[] All()
        {
            return byPeer.Values.ToArray();
        }
    }
}