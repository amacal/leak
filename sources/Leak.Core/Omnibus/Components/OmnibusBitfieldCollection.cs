using Leak.Common;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Omnibus.Components
{
    public class OmnibusBitfieldCollection
    {
        private readonly Dictionary<PeerHash, Bitfield> byPeer;
        private readonly OmnibusCache cache;

        private OmnibusBitfieldRanking ranking;

        public OmnibusBitfieldCollection(OmnibusCache cache)
        {
            this.cache = cache;
            this.byPeer = new Dictionary<PeerHash, Bitfield>();
        }

        public OmnibusBitfieldRanking Ranking
        {
            get { return ranking ?? (ranking = new OmnibusBitfieldRanking(cache, byPeer.Values.ToArray())); }
        }

        public void Add(PeerHash peer, Bitfield bitfield)
        {
            byPeer[peer] = bitfield;
            ranking = null;
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
    }
}