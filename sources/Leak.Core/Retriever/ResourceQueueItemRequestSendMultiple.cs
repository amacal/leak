using Leak.Core.Collector;
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
            PeerCollectorCriterion[] criterion =
            {
                PeerCollectorCriterion.IsLocalNotChokedByRemote,
            };

            //int slots = 8;

            foreach (PeerHash peer in context.Collector.GetPeers(criterion))
            {
                int pieces = 4;

                //if (slots > 0)// && peer.Rank > 128)
                //{
                //    slots--;
                //    pieces = 64;
                //}

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
                    context.Omnibus.Reserve(peer, block);
                }
            }
        }
    }
}