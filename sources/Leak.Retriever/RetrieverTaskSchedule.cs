using Leak.Common;
using Leak.Tasks;

namespace Leak.Dataget
{
    public class RetrieverTaskSchedule : LeakTask<RetrieverContext>
    {
        private readonly PeerHash peer;

        public RetrieverTaskSchedule(PeerHash peer)
        {
            this.peer = peer;
        }

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

            foreach (PeerHash found in omnibus.Find(ranking, count))
            {
                if (found.Equals(peer))
                {
                    omnibus.Schedule(strategy, found, pieces);
                }
            }
        }
    }
}