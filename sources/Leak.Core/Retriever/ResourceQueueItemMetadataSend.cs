using Leak.Core.Collector;
using Leak.Core.Common;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemMetadataSend : ResourceQueueItem
    {
        public void Handle(ResourceQueueContext context)
        {
            PeerCollectorCriterion[] criterion =
            {
                PeerCollectorCriterion.IsLocalNotChockedByRemote,
                PeerCollectorCriterion.DoesRemoteSupportMetadata
            };

            foreach (PeerHash peer in context.Collector.GetPeers(criterion))
            {
                ResourceMetadataBlock[] requests = context.Storage.ScheduleMetadata(peer);

                foreach (ResourceMetadataBlock request in requests)
                {
                    context.Storage.Reserve(peer, request);
                    context.Collector.SendMetadataRequest(peer, request.Index);
                }
            }
        }
    }
}