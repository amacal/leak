using System.Collections.Generic;
using System.Linq;
using Leak.Common;
using Leak.Events;
using Leak.Peer.Coordinator.Events;

namespace Leak.Data.Map.Components
{
    public class OmnibusBitfieldCollection
    {
        private readonly Dictionary<PeerHash, Bitfield> byPeer;
        private readonly OmnibusBitfieldRanking ranking;

        public OmnibusBitfieldCollection()
        {
            this.ranking = new OmnibusBitfieldRanking();
            this.byPeer = new Dictionary<PeerHash, Bitfield>();
        }

        public OmnibusBitfieldRanking Ranking
        {
            get { return ranking; }
        }

        public void Handle(BitfieldChanged data)
        {
            if (data.Bitfield != null)
            {
                byPeer[data.Peer] = data.Bitfield;
                ranking.Add(data.Bitfield);
            }

            if (data.Affected != null)
            {
                ranking.Add(data.Affected);
            }
        }

        public void Handle(PeerDisconnected data)
        {
            Bitfield bitfield;

            if (byPeer.TryGetValue(data.Peer, out bitfield))
            {
                ranking.Remove(bitfield);
            }
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