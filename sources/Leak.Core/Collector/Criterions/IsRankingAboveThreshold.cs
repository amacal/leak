using Leak.Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Collector.Criterions
{
    public class IsRankingAboveThreshold : PeerCollectorCriterion
    {
        private readonly int threshold;

        public IsRankingAboveThreshold(int threshold)
        {
            this.threshold = threshold;
        }

        public IEnumerable<PeerHash> Accept(IEnumerable<PeerHash> peers, PeerCollectorContext context)
        {
            List<Peer> result = new List<Peer>();

            foreach (PeerHash hash in peers)
            {
                Peer peer = new Peer
                {
                    Hash = hash,
                    Ranking = context.Ranking.Get(hash)
                };

                if (peer.Ranking >= threshold)
                {
                    result.Add(peer);
                }
            }

            return result.OrderByDescending(x => x.Ranking).Select(x => x.Hash);
        }

        private struct Peer
        {
            public PeerHash Hash;

            public int Ranking;
        }
    }
}