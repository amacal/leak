using Leak.Core.Collector;
using Leak.Core.Common;
using Leak.Core.Messages;
using System.Collections.Generic;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemRequestSendMultiple : ResourceQueueItem
    {
        public void Handle(ResourceQueueContext context)
        {
            PeerCollectorCriterion[] criterion =
            {
                PeerCollectorCriterion.IsLocalNotChockedByRemote,
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
                ResourceBlock[] blocks = context.Storage.Next(peer, pieces);

                foreach (ResourceBlock block in blocks)
                {
                    requests.Add(new Request(block.Index, block.Offset, block.Size));
                }

                context.Collector.SendPieceRequest(peer, requests.ToArray());

                foreach (ResourceBlock block in blocks)
                {
                    context.Storage.Reserve(peer, block);
                }
            }
        }
    }
}