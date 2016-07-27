using Leak.Core.Common;
using Leak.Core.Extensions;
using Leak.Core.Extensions.Metadata;
using Leak.Core.Messages;
using Leak.Core.Repository;
using System;
using System.Threading;

namespace Leak.Core.Retriever
{
    public class ResourceRetrieverToGet : ResourceRetriever
    {
        private readonly ResourceRetrieverConfiguration configuration;
        private readonly ResourceQueueContext context;
        private readonly ResourceQueue queue;

        private readonly Timer timer;
        private int tick;

        public ResourceRetrieverToGet(Action<ResourceRetrieverConfiguration> configurer)
        {
            this.configuration = configurer.Configure(with =>
            {
                with.Callback = new ResourceRetrieverToNothing();
            });

            this.context = new ResourceQueueContext
            {
                Callback = configuration.Callback,
                Collector = configuration.Collector,
                Extender = configuration.Extender,
                Repository = configuration.Repository,
                Storage = new ResourceStorage(new ResourceStorageConfiguration())
            };

            this.context.Storage = new ResourceStorage(new ResourceStorageConfiguration
            {
                Pieces = this.context.Repository.Properties.Pieces,
                Blocks = this.context.Repository.Properties.Blocks,
                BlocksInPiece = this.context.Repository.Properties.PieceSize / this.context.Repository.Properties.BlockSize,
                BlockSize = this.context.Repository.Properties.BlockSize,
                TotalSize = this.context.Repository.Properties.TotalSize
            });

            this.timer = new Timer(OnTick);
            this.timer.Change(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));

            this.queue = new ResourceQueue();
        }

        public ResourceRetrieverToGet(ResourceQueueContext context, ResourceQueue queue, Action<ResourceRetrieverConfiguration> configurer)
        {
            this.configuration = configurer.Configure(with =>
            {
                with.Callback = new ResourceRetrieverToNothing();
            });

            this.queue = queue;
            this.context = context;

            this.context.Callback = configuration.Callback;
            this.context.Collector = configuration.Collector;
            this.context.Extender = configuration.Extender;
            this.context.Repository = configuration.Repository;

            this.context.Storage = new ResourceStorage(this.context.Storage, new ResourceStorageConfiguration
            {
                Pieces = this.context.Repository.Properties.Pieces,
                Blocks = this.context.Repository.Properties.Blocks,
                BlocksInPiece = this.context.Repository.Properties.PieceSize / this.context.Repository.Properties.BlockSize,
                BlockSize = this.context.Repository.Properties.BlockSize,
                TotalSize = this.context.Repository.Properties.TotalSize
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
                    tick = (tick + 1) % 600;

                    if (tick == 0)
                    {
                        queue.Enqueue(new ResourceQueueItemKeepAliveSend());
                    }

                    queue.Enqueue(new ResourceQueueItemRequestSendMultiple());
                    queue.Process(context);
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

        public void SetExtensions(PeerHash peer, ExtenderHandshake handshake)
        {
            queue.Enqueue(new ResourceQueueItemHandshakeHandle(peer));
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