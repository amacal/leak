using Leak.Core.Collector;
using Leak.Core.Collector.Criterions;
using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Omnibus;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemRequestSendMultiple : ResourceQueueItem
    {
        public void Handle(ResourceQueueContext context)
        {
            Schedule(context, 1024, 8, 64);
            Schedule(context, 128, 8, 16);
            Schedule(context, 0, 16, 4);
        }

        private void Schedule(ResourceQueueContext context, int ranking, int count, int pieces)
        {
            PeerCollectorCriterion[] criterion =
            {
                new IsLocalNotChokedByRemote(),
                new IsRankingAboveThreshold(ranking),
            };

            foreach (PeerHash peer in context.Collector.GetPeers(criterion).Take(count))
            {
                List<Request> requests = new List<Request>();
                OmnibusStrategy strategy = OmnibusStrategy.Sequential;
                OmnibusBlock[] blocks = context.Omnibus.Next(strategy, peer, pieces).ToArray();

                foreach (OmnibusBlock block in blocks)
                {
                    requests.Add(new Request(block.Piece, block.Offset, block.Size));
                }

                context.Collector.SendPieceRequest(peer, requests.ToArray());

                foreach (OmnibusBlock block in blocks)
                {
                    PeerHash previous = context.Omnibus.Reserve(peer, block);

                    context.Collector.Decrease(peer, 1);

                    if (previous != null)
                    {
                        context.Collector.Decrease(previous, 20);
                    }
                }
            }
        }
    }
}