using Leak.Core.Common;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemMetadataSend : ResourceQueueItem
    {
        public void Handle(ResourceQueueContext context)
        {
            foreach (ResourcePeer peer in context.Storage.GetPeers(ResourcePeerOperation.Metadata))
            {
                PeerHash hash = peer.Hash;
                ResourceMetadataBlock[] requests = context.Storage.ScheduleMetadata(hash);

                foreach (ResourceMetadataBlock request in requests)
                {
                    context.Storage.Reserve(hash, request);
                    context.Collector.SendMetadataRequest(hash, request.Index);
                }
            }
        }
    }
}