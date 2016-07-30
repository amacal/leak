using Leak.Core.Common;
using Leak.Core.Extensions.Metadata;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemMetadataSend : ResourceQueueItem
    {
        public void Handle(ResourceQueueContext context)
        {
            foreach (ResourcePeer peer in context.Storage.GetPeers(ResourcePeerOperation.Metadata))
            {
                if (context.Extender.MetadataSupports(peer.Hash))
                {
                    PeerHash hash = peer.Hash;
                    ResourceMetadataBlock[] requests = context.Storage.ScheduleMetadata(hash);

                    foreach (ResourceMetadataBlock request in requests)
                    {
                        context.Storage.Reserve(hash, request);
                        context.Collector.SendExtended(hash, context.Extender.MetadataRequest(hash, request.Index));
                    }
                }
            }
        }
    }
}