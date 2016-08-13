using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Repository;
using System;
using System.Threading;

namespace Leak.Core.Retriever
{
    public class ResourceRetrieverToGet : ResourceRetriever
    {
        private readonly ResourceRetrieverConfiguration configuration;
        private readonly ResourceRepository repository;
        private readonly ResourceQueueContext context;
        private readonly ResourceQueue queue;

        private readonly Timer timer;

        public ResourceRetrieverToGet(Action<ResourceRetrieverConfiguration> configurer)
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
                Properties = repository.Properties,
                Storage = new ResourceStorage(new ResourceStorageConfiguration())
            };

            this.context.Storage = new ResourceStorage(new ResourceStorageConfiguration
            {
                Pieces = this.context.Properties.Pieces,
                PieceSize = this.context.Properties.PieceSize,
                BlockSize = this.context.Properties.BlockSize,
                TotalSize = this.context.Properties.TotalSize
            });

            this.timer = new Timer(OnTick);
            this.timer.Change(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
        }

        public ResourceRetrieverToGet(ResourceQueueContext context, ResourceQueue queue, ResourceRepository repository, Action<ResourceRetrieverConfiguration> configurer)
        {
            this.configuration = configurer.Configure(with =>
            {
                with.Callback = new ResourceRetrieverToNothing();
            });

            this.queue = queue;
            this.context = context;
            this.repository = repository;

            this.context.Callback = configuration.Callback;
            this.context.Collector = configuration.Collector;
            this.context.Properties = repository.Properties;

            this.context.Storage = new ResourceStorage(this.context.Storage, new ResourceStorageConfiguration
            {
                Pieces = this.context.Properties.Pieces,
                PieceSize = this.context.Properties.PieceSize,
                BlockSize = this.context.Properties.BlockSize,
                TotalSize = this.context.Properties.TotalSize
            });

            this.timer = new Timer(OnTick);
            this.timer.Change(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
        }

        private void OnTick(object state)
        {
            timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            try
            {
                lock (context.Storage)
                {
                    using (ResourceRepositorySession session = repository.OpenSession())
                    {
                        queue.Enqueue(new ResourceQueueItemRequestSendMultiple());
                        queue.Process(context.Configure(with =>
                        {
                            with.Repository = session;
                        }));
                    }
                }
            }
            finally
            {
                this.timer.Change(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
            }
        }

        public ResourceRetriever WithBitfield(Bitfield bitfield)
        {
            lock (context.Storage)
            {
                context.Storage.Complete(bitfield);
            }

            return this;
        }

        public ResourceRetriever WithRepository(ResourceRepository repository)
        {
            throw new NotImplementedException();
        }

        public void SetBitfield(PeerHash peer, Bitfield bitfield)
        {
            queue.Enqueue(new ResourceQueueItemBitfieldHandle(peer, bitfield));
        }

        public void SetChoked(PeerHash peer, ResourceDirection direction)
        {
            queue.Enqueue(new ResourceQueueItemChokeHandle(peer));
        }

        public void SetUnchoked(PeerHash peer, ResourceDirection direction)
        {
            queue.Enqueue(new ResourceQueueItemUnchokeHandle(peer));
        }

        public void AddPiece(PeerHash peer, Piece piece)
        {
            queue.Enqueue(new ResourceQueueItemPieceHandle(peer, piece));
            queue.Enqueue(new ResourceQueueItemRequestSend(peer));
        }

        public void AddMetadata(PeerHash peer, MetadataData data)
        {
        }
    }
}