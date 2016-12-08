using Leak.Common;
using System.Collections.Generic;

namespace Leak.Core.Ranking
{
    public class RankingCollection
    {
        private readonly Dictionary<PeerHash, RankingEntry> byPeer;

        public RankingCollection()
        {
            byPeer = new Dictionary<PeerHash, RankingEntry>();
        }

        public RankingEntry GetOrCreate(PeerHash peer)
        {
            RankingEntry entry;

            if (byPeer.TryGetValue(peer, out entry) == false)
            {
                entry = new RankingEntry(peer);
                byPeer.Add(peer, entry);
            }

            return entry;
        }
    }
}