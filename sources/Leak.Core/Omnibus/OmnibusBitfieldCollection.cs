using Leak.Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Omnibus
{
    public class OmnibusBitfieldCollection
    {
        private readonly int size;
        private readonly Dictionary<PeerHash, Bitfield> byPeer;

        private OmnibusBitfieldRanking ranking;

        public OmnibusBitfieldCollection(int size)
        {
            this.size = size;
            this.byPeer = new Dictionary<PeerHash, Bitfield>();
        }

        public int Size
        {
            get { return size; }
        }

        public OmnibusBitfieldRanking Ranking
        {
            get { return ranking ?? (ranking = new OmnibusBitfieldRanking(byPeer.Values.ToArray(), size)); }
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