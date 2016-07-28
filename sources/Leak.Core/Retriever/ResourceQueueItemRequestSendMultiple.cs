using Leak.Core.Common;
using Leak.Core.Messages;
using System.Collections.Generic;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemRequestSendMultiple : ResourceQueueItem
    {
        public void Handle(ResourceQueueContext context)
        {
            int slots = 8;

            foreach (ResourcePeer peer in context.Storage.GetPeers(ResourcePeerOperation.Metadata))
            {
                int pieces = 4;

                if (slots > 0 && peer.Rank > 128)
                {
                    slots--;
                    pieces = 64;
                }

                PeerHash hash = peer.Hash;
                List<Request> requests = new List<Request>();
                ResourceBlock[] blocks = context.Storage.Next(hash, pieces);

                foreach (ResourceBlock block in blocks)
                {
                    requests.Add(new Request(block.Index, block.Offset, block.Size));
                }

                context.Collector.SendPieceRequest(hash, requests.ToArray());

                foreach (ResourceBlock block in blocks)
                {
                    context.Storage.Reserve(hash, block);
                }
            }
        }
    }
}