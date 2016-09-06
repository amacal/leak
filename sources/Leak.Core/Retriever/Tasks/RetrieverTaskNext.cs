using Leak.Core.Collector;
using Leak.Core.Collector.Criterions;
using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Omnibus;
using System;
using System.Linq;

namespace Leak.Core.Retriever.Tasks
{
    public class RetrieverTaskNext : LeakTask<RetrieverContext>
    {
        public void Execute(RetrieverContext context)
        {
            Schedule(context, 1024, 8, 64);
            Schedule(context, 128, 8, 16);
            Schedule(context, 0, 16, 4);
        }

        private void Schedule(RetrieverContext context, int ranking, int count, int pieces)
        {
            PeerCollectorCriterion[] criterion =
            {
                new IsLocalNotChokedByRemote(),
                new OrderByRanking(ranking),
            };

            foreach (PeerSession session in context.Collector.GetPeers(criterion).Take(count))
            {
                context.Omnibus.Schedule(OmnibusStrategy.RarestFirst, session.Peer, pieces);
            }

            context.NextSchedule = DateTime.Now.AddSeconds(0.25);
        }
    }
}