using Leak.Core.Collector;
using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Repository;
using System;
using System.Threading;

namespace Leak.Core.Retriever
{
    public class ResourceRetriever
    {
        private readonly ResourceRetrieverConfiguration configuration;
        private readonly ResourceRetrieverCallback callback;
        private readonly ResourceRepository repository;
        private readonly PeerCollectorView collector;
        private readonly ResourceStorage storage;

        private readonly Timer timer;
        private int tick;

        public ResourceRetriever(Action<ResourceRetrieverConfiguration> configurer)
        {
            this.configuration = configurer.Configure(with =>
            {
                with.Callback = new ResourceRetrieverToNothing();
            });

            this.repository = configuration.Repository;
            this.collector = configuration.Collector;
            this.callback = configuration.Callback;

            this.storage = new ResourceStorage(new ResourceStorageConfiguration
            {
                Pieces = repository.Properties.Pieces,
                Blocks = repository.Properties.Blocks,
                BlocksInPiece = repository.Properties.PieceSize / repository.Properties.BlockSize,
                BlockSize = repository.Properties.BlockSize,
                TotalSize = repository.Properties.TotalSize
            });

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

                foreach (ResourcePeer peer in storage.GetPeers(ResourcePeerOperation.Request))
                {
                    SendPieceRequests(peer.Hash);
                }
            }
        }

        public void Initialize(Bitfield bitfield)
        {
            lock (storage)
            {
                storage.Complete(bitfield);
            }
        }

        public void Start(PeerHash peer, Bitfield bitfield)
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
            lock (storage)
            {
                if (storage.IsComplete(piece.Index) == false)
                {
                    int blockIndex = piece.Offset / repository.Properties.BlockSize;
                    ResourceBlock block = new ResourceBlock(piece.Index, piece.Offset, piece.Size);

                    repository.SetPiece(piece.Index, blockIndex, piece.Data);
                    bool completed = storage.Complete(peer, block);

                    if (completed)
                    {
                        if (repository.Verify(piece.Index) == false)
                        {
                            storage.Invalidate(piece.Index);
                        }
                        else
                        {
                            callback.OnPieceVerified(new ResourcePiece(piece.Index));
                        }
                    }

                    if (storage.IsComplete())
                    {
                        callback.OnCompleted();
                    }
                }

                SendPieceRequests(peer);
            }
        }

        private void SendPieceRequests(PeerHash peer)
        {
            ResourceBlock[] blocks = storage.Next(peer);
            foreach (ResourceBlock block in blocks)
            {
                collector.SendPieceRequest(peer, block.Index, block.Offset, block.Size);
                storage.Book(peer, block);
            }
        }
    }
}