using Leak.Core.Collector;
using System;

namespace Leak.Core.Scheduler
{
    public class SchedulerContext
    {
        private readonly SchedulerConfiguration configuration;
        private readonly SchedulerQueue queue;

        public SchedulerContext(Action<SchedulerConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new SchedulerCallbackNothing();
            });

            queue = new SchedulerQueue(this);
        }

        public SchedulerCallback Callback
        {
            get { return configuration.Callback; }
        }

        public PeerCollector Collector
        {
            get { return configuration.Collector; }
        }

        public SchedulerQueue Queue
        {
            get { return queue; }
        }
    }
}