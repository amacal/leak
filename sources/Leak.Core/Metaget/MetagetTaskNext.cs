using Leak.Core.Collector;
using Leak.Core.Collector.Criterions;
using Leak.Core.Common;
using Leak.Core.Metamine;

namespace Leak.Core.Metaget
{
    public class MetagetTaskNext : MetagetTask
    {
        private static readonly PeerCollectorCriterion[] Criterions =
        {
            new IsLocalNotChokedByRemote(),
            new IsMetadataSupportedByRemote()
        };

        public void Execute(MetagetContext context)
        {
            if (context.Metamine != null)
            {
                foreach (PeerHash peer in context.View.GetPeers(Criterions))
                {
                    MetamineStrategy strategy = MetamineStrategy.Sequential;
                    MetamineBlock[] blocks = context.Metamine.Next(strategy, peer);

                    foreach (MetamineBlock block in blocks)
                    {
                        context.Metamine.Reserve(peer, block);
                        context.View.SendMetadataRequest(peer, block.Index);
                        context.Callback.OnMetadataRequested(peer, block.Index);
                    }
                }
            }
        }
    }
}