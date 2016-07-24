using Leak.Core.Collector;
using Leak.Core.Common;
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
        private readonly ResourceRetrieverCallback callback;
        private readonly PeerCollectorView collector;
        private readonly ResourceStorage storage;

        private readonly Timer timer;
        private int tick;

        public ResourceRetrieverToQuery(Action<ResourceRetrieverConfiguration> configurer)
        {
            this.configuration = configurer.Configure(with =>
            {
                with.Callback = new ResourceRetrieverToNothing();
            });

            this.collector = configuration.Collector;
            this.callback = configuration.Callback;
            this.storage = new ResourceStorage(new ResourceStorageConfiguration());

            timer = new Timer(OnTick);
            timer.Change(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
        }

        private void OnTick(object state)
        {
            lock (storage)
            {
                tick = (tick + 1) % 20;

                if (tick == 0)
                {
                    foreach (ResourcePeer peer in storage.GetPeers(ResourcePeerOperation.KeepAlive))
                    {
                        collector.SendKeepAlive(peer.Hash);
                    }
                }

                foreach (ResourcePeer peer in storage.GetPeers(ResourcePeerOperation.Metadata))
                {
                    SendMetadataRequest(peer.Hash);
                }
            }
        }

        public ResourceRetriever WithBitfield(Bitfield bitfield)
        {
            lock (storage)
            {
                storage.Complete(bitfield);
            }

            return this;
        }

        public ResourceRetriever WithRepository(ResourceRepository repository)
        {
            lock (storage)
            {
                timer.Dispose();

                return new ResourceRetrieverToGet(storage, with =>
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
            lock (storage)
            {
                storage.AddPeer(peer);
                collector.SendExtended(peer, configuration.Extender.GetHandshake());
            }
        }

        public void SetBitfield(PeerHash peer, Bitfield bitfield)
        {
            lock (storage)
            {
                storage.AddPeer(peer);
                storage.AddBitfield(peer, bitfield);
                collector.SendInterested(peer);
            }
        }

        public void SetChoked(PeerHash peer, ResourceDirection direction)
        {
            lock (storage)
            {
                storage.Choke(peer);
            }
        }

        public void SetUnchoked(PeerHash peer, ResourceDirection direction)
        {
            lock (storage)
            {
                storage.Unchoke(peer);
            }
        }

        public void AddPiece(PeerHash peer, Piece piece)
        {
        }

        public void AddMetadata(PeerHash peer, MetadataData data)
        {
            ResourceMetadataBlock block = new ResourceMetadataBlock(data.Piece);

            lock (storage)
            {
                if (storage.IsMetadataComplete() == false)
                {
                    storage.Complete(peer, block, data.Size);

                    if (configuration.Repository.SetMetadata(data.Piece, data.Payload))
                    {
                        storage.Complete(data.Size);
                        configuration.Callback.OnMetadataCompleted();
                    }
                }
            }
        }

        private void SendMetadataRequest(PeerHash peer)
        {
            ResourceMetadataBlock[] requests = storage.ScheduleMetadata(peer);

            foreach (ResourceMetadataBlock request in requests)
            {
                storage.Reserve(peer, request);
                collector.SendExtended(peer, configuration.Extender.MetadataRequest(peer, request.Index));
            }
        }
    }
}