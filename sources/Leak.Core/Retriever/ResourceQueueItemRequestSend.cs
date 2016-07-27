using Leak.Core.Common;

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
            ResourceBlock[] blocks = context.Storage.Next(peer);

            foreach (ResourceBlock block in blocks)
            {
                context.Collector.SendPieceRequest(peer, block.Index, block.Offset, block.Size);
                context.Storage.Reserve(peer, block);
            }
        }
    }
}