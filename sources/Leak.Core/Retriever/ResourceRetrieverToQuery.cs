﻿using Leak.Core.Common;
using Leak.Core.Extensions;
using Leak.Core.Extensions.Metadata;
using Leak.Core.Messages;
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

        private readonly Timer timer;
        private int tick;

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
                Extender = configuration.Extender,
                Storage = new ResourceStorage(new ResourceStorageConfiguration())
            };

            this.timer = new Timer(OnTick);
            this.timer.Change(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
        }

        private void OnTick(object state)
        {
            lock (context.Storage)
            {
                using (ResourceRepositorySession session = repository.OpenSession())
                {
                    tick = (tick + 1) % 20;

                    if (tick == 0)
                    {
                        queue.Enqueue(new ResourceQueueItemKeepAliveSend());
                    }

                    queue.Enqueue(new ResourceQueueItemMetadataSend());
                    queue.Process(context.Configure(with =>
                    {
                        with.Repository = session;
                    }));
                }
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
            lock (context.Storage)
            {
                timer.Dispose();

                return new ResourceRetrieverToGet(context, queue, repository, with =>
                {
                    with.Callback = configuration.Callback;
                    with.Extender = configuration.Extender;
                    with.Collector = configuration.Collector;
                    with.Repository = repository;
                });
            }
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
            queue.Enqueue(new ResourceQueueItemHandshakeHandle(peer));
            queue.Enqueue(new ResourceQueueItemUnchokeHandle(peer));
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