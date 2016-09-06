using System;
using Leak.Core.Collector;
using Leak.Core.Core;
using Leak.Core.Metadata;
using Leak.Core.Omnibus;
using Leak.Core.Repository;
using Leak.Core.Retriever.Callbacks;

namespace Leak.Core.Retriever.Components
{
    public class RetrieverContext
    {
        private readonly RetrieverConfiguration configuration;
        private readonly RepositoryService repository;
        private readonly OmnibusService omnibus;
        private readonly LeakQueue<RetrieverContext> queue;
        private readonly LeakTimer timer;

        private DateTime nextSchedule;

        public RetrieverContext(Action<RetrieverConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new RetrieverCallbackNothing();
            });

            repository = new RepositoryService(with =>
            {
                with.Metainfo = configuration.Metainfo;
                with.Destination = configuration.Destination;
                with.Callback = new RetrieverToRepository(this);
            });

            omnibus = new OmnibusService(with =>
            {
                with.Metainfo = configuration.Metainfo;
                with.Bitfield = configuration.Bitfield;
                with.Callback = new RetrieverToOmnibus(this);
            });

            queue = new LeakQueue<RetrieverContext>();
            timer = new LeakTimer(TimeSpan.FromMilliseconds(25));
        }

        public RetrieverConfiguration Configuration
        {
            get { return configuration; }
        }

        public PeerCollectorView Collector
        {
            get { return configuration.Collector; }
        }

        public RetrieverCallback Callback
        {
            get { return configuration.Callback; }
        }

        public Metainfo Metainfo
        {
            get { return configuration.Metainfo; }
        }

        public RepositoryService Repository
        {
            get { return repository; }
        }

        public LeakQueue<RetrieverContext> Queue
        {
            get { return queue; }
        }

        public LeakTimer Timer
        {
            get { return timer; }
        }

        public OmnibusService Omnibus
        {
            get { return omnibus; }
        }

        public DateTime NextSchedule
        {
            get { return nextSchedule; }
            set { nextSchedule = value; }
        }
    }
}