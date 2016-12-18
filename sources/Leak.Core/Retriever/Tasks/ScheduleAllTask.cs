using Leak.Common;
using Leak.Core.Retriever.Components;
using Leak.Omnibus;
using Leak.Tasks;

namespace Leak.Core.Retriever.Tasks
{
    public class ScheduleAllTask : LeakTask<RetrieverContext>
    {
        public void Execute(RetrieverContext context)
        {
            Schedule(context, 2048, 8, 256);
            Schedule(context, 1024, 8, 64);
            Schedule(context, 128, 8, 16);
            Schedule(context, 0, 16, 4);
        }

        private void Schedule(RetrieverContext context, int ranking, int count, int pieces)
        {
            OmnibusStrategy strategy = context.Configuration.Strategy.ToOmnibus();

            foreach (PeerHash peer in context.Omnibus.Find(ranking, count))
            {
                context.Omnibus.Schedule(strategy, peer, pieces);
            }
        }
    }
}