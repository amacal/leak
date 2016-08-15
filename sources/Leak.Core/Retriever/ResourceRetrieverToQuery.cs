using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Metamine;
using Leak.Core.Omnibus;
using Leak.Core.Repository;
using System;
using System.Threading;

namespace Leak.Core.Retriever
{
    public class ResourceRetrieverToQuery : ResourceRetriever
    {
        private readonly ResourceRetrieverConfiguration configuration;
        private readonly ResourceRepository repository;
        private readonly ResourceQueueContext context;
        private readonly ResourceQueue queue;

        private Timer timer;

        public ResourceRetrieverToQuery(Action<ResourceRetrieverConfiguration> configurer)
        {
            this.configuration = configurer.Configure(with =>
            {
                with.Callback = new ResourceRetrieverToNothing();
            });

            this.repository = configuration.Repository;
            this.queue = new ResourceQueue();

            this.context = new ResourceQueueContext
            {
                Callback = configuration.Callback,
                Collector = configuration.Collector,
            };

            this.context.Omnibus = new OmnibusBitfield(new OmnibusConfiguration());

            this.timer = new Timer(OnTick);
            this.timer.Change(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
        }

        private void OnTick(object state)
        {
            timer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            try
            {
                using (ResourceRepositorySession session = repository.OpenSession())
                {
                    queue.Enqueue(new ResourceQueueItemMetadataSend());
                    queue.Process(context.Configure(with =>
                    {
                        with.Repository = session;
                    }));
                }
            }
            finally
            {
                timer?.Change(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
            }
        }

        public ResourceRetriever WithBitfield(Bitfield bitfield)
        {
            context.Omnibus.Complete(bitfield);
            return this;
        }

        public ResourceRetriever WithMetadata(MetadataSize size)
        {
            lock (context)
            {
                if (context.Metamine == null)
                {
                    context.Metamine = new MetamineBitfield(with =>
                    {
                        with.Size = size.Bytes;
                    });
                }
            }

            return this;
        }

        public ResourceRetriever WithRepository(ResourceRepository repository)
        {
            lock (context)
            {
                timer?.Dispose();
                timer = null;

                return new ResourceRetrieverToGet(context, queue, repository, with =>
                {
                    with.Callback = configuration.Callback;
                    with.Collector = configuration.Collector;
                    with.Repository = repository;
                });
            }
        }

        public void SetBitfield(PeerHash peer, Bitfield bitfield)
        {
            queue.Enqueue(new ResourceQueueItemBitfieldHandle(peer, bitfield));
        }

        public void AddPiece(PeerHash peer, Piece piece)
        {
        }

        public void AddMetadata(PeerHash peer, MetadataData data)
        {
            queue.Enqueue(new ResourceQueueItemMetadataHandle(peer, data));
        }
    }
}