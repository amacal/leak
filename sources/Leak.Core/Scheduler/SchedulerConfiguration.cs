using Leak.Core.Collector;

namespace Leak.Core.Scheduler
{
    public class SchedulerConfiguration
    {
        public PeerCollector Collector { get; set; }

        public SchedulerCallback Callback { get; set; }
    }
}