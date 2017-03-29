using Leak.Common;
using Leak.Tasks;

namespace Leak.Data.Get
{
    public class DataGetTaskScheduleAll : LeakTask<DataGetContext>
    {
        public void Execute(DataGetContext context)
        {
            Schedule(context, 2048, 8, 256);
            Schedule(context, 1024, 8, 64);
            Schedule(context, 128, 8, 16);
            Schedule(context, 0, 16, 4);
        }

        private void Schedule(DataGetContext context, int ranking, int count, int pieces)
        {
            string strategy = context.Configuration.Strategy;
            DataGetToDataMap omnibus = context.Dependencies.DataMap;

            foreach (PeerHash peer in omnibus.Find(ranking, count))
            {
                omnibus.Schedule(strategy, peer, pieces);
            }
        }
    }
}