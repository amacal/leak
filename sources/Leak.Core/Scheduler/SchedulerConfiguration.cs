using Leak.Core.Collector;
using Leak.Core.Retriever;

namespace Leak.Core.Scheduler
{
    public class SchedulerConfiguration
    {
        public PeerCollector Collector { get; set; }

        public SchedulerCallback Callback { get; set; }

        public RetrieverStrategy Strategy { get; set; }
    }
}