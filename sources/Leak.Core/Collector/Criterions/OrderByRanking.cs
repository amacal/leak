using Leak.Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Collector.Criterions
{
    public class OrderByRanking : PeerCollectorCriterion
    {
        private readonly int threshold;

        public OrderByRanking(int threshold)
        {
            this.threshold = threshold;
        }

        public IEnumerable<PeerSession> Accept(IEnumerable<PeerSession> sessions, PeerCollectorContext context)
        {
            List<Peer> result = new List<Peer>();

            foreach (PeerSession session in sessions)
            {
                Peer peer = new Peer
                {
                    Session = session,
                    Ranking = context.Ranking.Get(session.Peer)
                };

                if (peer.Ranking >= threshold)
                {
                    result.Add(peer);
                }
            }

            return result.OrderByDescending(x => x.Ranking).Select(x => x.Session);
        }

        private struct Peer
        {
            public PeerSession Session;

            public int Ranking;
        }
    }
}