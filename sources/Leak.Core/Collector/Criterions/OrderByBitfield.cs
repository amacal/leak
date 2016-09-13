using Leak.Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Collector.Criterions
{
    public class OrderByBitfield : PeerCollectorCriterion
    {
        public IEnumerable<PeerSession> Accept(IEnumerable<PeerSession> sessions, PeerCollectorContext context)
        {
            List<Peer> result = new List<Peer>();

            foreach (PeerSession session in sessions)
            {
                Peer peer = new Peer
                {
                    Session = session,
                    Completed = context.Battlefield.Get(session)?.Completed
                };

                if (peer.Completed != null)
                {
                    result.Add(peer);
                }
            }

            return result.OrderByDescending(x => x.Completed.Value).Select(x => x.Session);
        }

        private struct Peer
        {
            public PeerSession Session;

            public int? Completed;
        }
    }
}