using Leak.Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Omnibus.Components
{
    public class OmnibusBitfieldCollection
    {
        private readonly int size;
        private readonly Dictionary<PeerHash, Bitfield> byPeer;
        private readonly OmnibusBitfieldCache cache;

        private OmnibusBitfieldRanking ranking;

        public OmnibusBitfieldCollection(int size)
        {
            this.size = size;
            this.byPeer = new Dictionary<PeerHash, Bitfield>();
            this.cache = new OmnibusBitfieldCache(size);
        }

        public int Size
        {
            get { return size; }
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