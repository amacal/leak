using Leak.Common;
using Leak.Retriever.Components;
using Leak.Tasks;

namespace Leak.Retriever.Tasks
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
            string strategy = context.Configuration.Strategy;
            RetrieverOmnibus omnibus = context.Dependencies.Omnibus;

            foreach (PeerHash peer in omnibus.Find(ranking, count))
            {
                omnibus.Schedule(strategy, peer, pieces);
            }
        }
    }
}