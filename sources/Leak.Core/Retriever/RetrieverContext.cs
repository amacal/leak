using Leak.Core.Collector;
using Leak.Core.Messages;
using Leak.Core.Metadata;
using Leak.Core.Omnibus;
using Leak.Core.Repository;
using System;

namespace Leak.Core.Retriever
{
    public class RetrieverContext
    {
        private readonly RetrieverConfiguration configuration;
        private readonly RepositoryService repository;
        private readonly OmnibusBitfield omnibus;
        private readonly RetrieverQueue queue;
        private readonly RetrieverTimer timer;

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

            omnibus = new OmnibusBitfield(new OmnibusConfiguration
            {
                Metainfo = configuration.Metainfo
            });

            queue = new RetrieverQueue();
            timer = new RetrieverTimer(TimeSpan.FromSeconds(0.25));
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

        public Bitfield Bitfield
        {
            get { return configuration.Bitfield; }
        }

        public RepositoryService Repository
        {
            get { return repository; }
        }

        public RetrieverQueue Queue
        {
            get { return queue; }
        }

        public RetrieverTimer Timer
        {
            get { return timer; }
        }

        public OmnibusBitfield Omnibus
        {
            get { return omnibus; }
        }
    }
}