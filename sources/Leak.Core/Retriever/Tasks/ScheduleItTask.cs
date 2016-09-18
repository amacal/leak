using Leak.Core.Collector;
using Leak.Core.Collector.Criterions;
using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Retriever.Components;

namespace Leak.Core.Retriever.Tasks
{
    public class ScheduleItTask : LeakTask<RetrieverContext>
    {
        private readonly PeerHash peer;

        public ScheduleItTask(PeerHash peer)
        {
            this.peer = peer;
        }

        public void Execute(RetrieverContext context)
        {
            PeerCollectorCriterion[] criterion =
            {
                new IsLocalNotChokedByRemote()
            };

            if (context.Collector.Is(peer, criterion))
            {
                context.Omnibus.Schedule(context.Configuration.Strategy.ToOmnibus(), peer, 64);
            }
        }
    }
}