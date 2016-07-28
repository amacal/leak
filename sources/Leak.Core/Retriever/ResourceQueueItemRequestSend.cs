using Leak.Core.Common;
using Leak.Core.Messages;
using System.Collections.Generic;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemRequestSend : ResourceQueueItem
    {
        private readonly PeerHash peer;

        public ResourceQueueItemRequestSend(PeerHash peer)
        {
            this.peer = peer;
        }

        public void Handle(ResourceQueueContext context)
        {
            List<Request> requests = new List<Request>();
            ResourceBlock[] blocks = context.Storage.Next(peer, 16);

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