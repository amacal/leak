using Leak.Common;
using Leak.Tasks;

namespace Leak.Data.Get
{
    public class DataGetTaskSchedule : LeakTask<DataGetContext>
    {
        private readonly PeerHash peer;

        public DataGetTaskSchedule(PeerHash peer)
        {
            this.peer = peer;
        }

        public void Execute(DataGetContext context)
        {
            Schedule(context, 2048, 4, 256);
            Schedule(context, 1024, 6, 64);
            Schedule(context, 128, 8, 16);
            Schedule(context, 0, 10, 4);
        }

        private void Schedule(DataGetContext context, int ranking, int count, int pieces)
        {
            string strategy = context.Configuration.Strategy;
            DataGetToDataMap omnibus = context.Dependencies.DataMap;

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