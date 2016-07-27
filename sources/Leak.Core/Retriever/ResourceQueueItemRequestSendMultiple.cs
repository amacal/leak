using Leak.Core.Common;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemRequestSendMultiple : ResourceQueueItem
    {
        public void Handle(ResourceQueueContext context)
        {
            foreach (ResourcePeer peer in context.Storage.GetPeers(ResourcePeerOperation.Metadata))
            {
                PeerHash hash = peer.Hash;
                ResourceBlock[] blocks = context.Storage.Next(hash);

                foreach (ResourceBlock block in blocks)
                {
                    context.Collector.SendPieceRequest(hash, block.Index, block.Offset, block.Size);
                    context.Storage.Reserve(hash, block);
                }
            }
        }
    }
}