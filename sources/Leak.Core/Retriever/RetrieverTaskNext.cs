using Leak.Core.Collector;
using Leak.Core.Collector.Criterions;
using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Messages;
using Leak.Core.Omnibus;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Retriever
{
    public class RetrieverTaskNext : LeakTask<RetrieverContext>
    {
        public void Execute(RetrieverContext context)
        {
            Schedule(context, 1024, 8, 64);
            Schedule(context, 128, 8, 16);
            Schedule(context, 0, 16, 4);
        }

        private void Schedule(RetrieverContext context, int ranking, int count, int pieces)
        {
            PeerCollectorCriterion[] criterion =
            {
                new IsLocalNotChokedByRemote(),
                new OrderByRanking(ranking),
            };

            foreach (PeerSession session in context.Collector.GetPeers(criterion).Take(count))
            {
                List<Request> requests = new List<Request>();
                OmnibusStrategy strategy = OmnibusStrategy.RarestFirst;
                OmnibusBlock[] blocks = context.Omnibus.Next(strategy, session.Peer, pieces).ToArray();

                foreach (OmnibusBlock block in blocks)
                {
                    requests.Add(new Request(block.Piece, block.Offset, block.Size));
                }

                if (requests.Count > 0)
                {
                    context.Collector.SendPieceRequest(session.Peer, requests.ToArray());
                }

                foreach (OmnibusBlock block in blocks)
                {
                    PeerHash previous = context.Omnibus.Reserve(session.Peer, block);

                    context.Collector.Decrease(session.Peer, 1);

                    if (previous != null)
                    {
                        context.Collector.Decrease(previous, 20);
                    }
                }
            }
        }
    }
}