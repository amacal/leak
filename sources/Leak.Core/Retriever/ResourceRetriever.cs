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
        private readonly ResourceRepository repository;
        private readonly PeerCollectorView collector;
        private readonly ResourceStorage storage;

        private readonly Timer timer;

        public ResourceRetriever(ResourceRepository repository, PeerCollectorView collector)
        {
            this.repository = repository;
            this.collector = collector;

            this.storage = new ResourceStorage(new ResourceConfiguration
            {
                Pieces = repository.Properties.Pieces,
                Blocks = repository.Properties.Blocks,
                BlocksInPiece = repository.Properties.PieceSize / repository.Properties.BlockSize,
                BlockSize = repository.Properties.BlockSize,
                TotalSize = repository.Properties.TotalSize
            });

            timer = new Timer(OnTick);
            timer.Change(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        }

        private void OnTick(object state)
        {
            lock (storage)
            {
                foreach (PeerHash peer in storage.GetPeers())
                {
                    SendPieceRequests(peer);
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
                storage.AddBitfield(peer, bitfield);
                collector.SendInterested(peer);
            }
        }

        public void SetUnchoked(PeerHash peer, ResourceDirection direction)
        {
            lock (storage)
            {
                storage.AddPeer(peer);
                SendPieceRequests(peer);
            }
        }

        public void AddPiece(PeerHash peer, Piece piece)
        {
            lock (storage)
            {
                if (storage.IsComplete(piece.Index) == false)
                {
                    int block = piece.Offset / repository.Properties.BlockSize;
                    ResourcePieceRequest request = new ResourcePieceRequest(piece.Index, piece.Offset, piece.Size);

                    repository.SetPiece(piece.Index, block, piece.Data);
                    bool completed = storage.Complete(request);

                    if (completed)
                    {
                        if (repository.Verify(piece.Index) == false)
                        {
                            storage.Invalidate(piece.Index);
                        }
                    }
                }

                SendPieceRequests(peer);
            }
        }

        private void SendPieceRequests(PeerHash peer)
        {
            ResourcePieceRequest[] requests = storage.Next(peer);
            foreach (ResourcePieceRequest request in requests)
            {
                collector.SendPieceRequest(peer, request.Index, request.Offset, request.Size);
                storage.Book(peer, request);
            }
        }
    }
}