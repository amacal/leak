using Leak.Core.Collector;
using Leak.Core.Collector.Criterions;
using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Metamine;

namespace Leak.Core.Metaget
{
    public class MetagetTaskNext : LeakTask<MetagetContext>
    {
        private static readonly PeerCollectorCriterion[] Criterions =
        {
            new IsMetadataSupportedByRemote(),
            new OrderByBitfield()
        };

        public void Execute(MetagetContext context)
        {
            if (context.Metamine != null && context.Metafile.IsCompleted() == false)
            {
                foreach (PeerSession session in context.View.GetPeers(Criterions))
                {
                    MetamineStrategy strategy = MetamineStrategy.Sequential;
                    MetamineBlock[] blocks = context.Metamine.Next(strategy, session.Peer);

                    foreach (MetamineBlock block in blocks)
                    {
                        context.Metamine.Reserve(session.Peer, block);
                        context.View.SendMetadataRequest(session, block.Index);
                        context.Callback.OnMetadataRequested(session.Peer, block.Index);
                    }
                }
            }
        }
    }
}