using Leak.Core.Collector;
using Leak.Core.Collector.Criterions;
using Leak.Core.Common;
using Leak.Core.Metamine;
using System.Linq;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemMetadataSend : ResourceQueueItem
    {
        public void Handle(ResourceQueueContext context)
        {
            PeerCollectorCriterion[] criterion =
            {
                new IsLocalNotChokedByRemote(),
                new IsMetadataSupportedByRemote(),
            };

            if (context.Metamine != null)
            {
                foreach (PeerHash peer in context.Collector.GetPeers(criterion))
                {
                    MetamineStrategy strategy = MetamineStrategy.Sequential;
                    MetamineBlock[] blocks = context.Metamine.Next(strategy, peer).ToArray();

                    foreach (MetamineBlock block in blocks)
                    {
                        context.Collector.SendMetadataRequest(peer, block.Index);
                        context.Metamine.Reserve(peer, block);
                    }
                }
            }
        }
    }
}