using Leak.Core.Retriever;
using Leak.Core.Scheduler;

namespace Leak.Core.Client.Configuration
{
    public class PeerClientDownloadBuilder
    {
        private RetrieverStrategy? strategy;

        public void RarestFirst()
        {
            strategy = RetrieverStrategy.RarestFirst;
        }

        public void Sequential()
        {
            strategy = RetrieverStrategy.Sequential;
        }

        public void Apply(SchedulerConfiguration configuration)
        {
            if (strategy != null)
            {
                configuration.Strategy = strategy.Value;
            }
        }
    }
}