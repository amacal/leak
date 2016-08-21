using Leak.Core.Collector;
using System;

namespace Leak.Core.Tasking
{
    public class PeerClientTaskContext
    {
        private readonly PeerClientTaskConfiguration configuration;
        private readonly PeerClientTaskQueue queue;

        public PeerClientTaskContext(Action<PeerClientTaskConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
            });

            queue = new PeerClientTaskQueue(this);
        }

        public PeerClientTaskConfiguration Configuration
        {
            get { return configuration; }
        }

        public PeerCollector Collector
        {
            get { return configuration.Collector; }
        }

        public PeerClientTaskQueue Queue
        {
            get { return queue; }
        }
    }
}