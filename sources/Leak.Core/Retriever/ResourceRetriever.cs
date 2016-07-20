using Leak.Core.Collector;
using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Repository;

namespace Leak.Core.Retriever
{
    public class ResourceRetriever
    {
        private readonly ResourceRepository repository;
        private readonly PeerCollectorView collector;
        private readonly ResourceStorage storage;

        public ResourceRetriever(ResourceRepository repository, PeerCollectorView collector)
        {
            this.repository = repository;
            this.collector = collector;

            this.storage = new ResourceStorage(new ResourceConfiguration
            {
                Pieces = repository.Properties.Pieces,
                Blocks = repository.Properties.Blocks,
                BlocksInPiece = repository.Properties.PieceSize / repository.Properties.BlockSize,
                BlockSize = repository.Properties.BlockSize
            });
        }

        public void Start(PeerHash peer, Bitfield bitfield)
        {
            lock (storage)
            {
                storage.AddBitfield(peer, bitfield);
                collector.SendInterested(peer);
            }
        }

        public void Unchoke(PeerHash peer, ResourceDirection direction)
        {
            lock (storage)
            {
                ResourcePieceRequest[] requests = storage.Next(peer);

                foreach (ResourcePieceRequest request in requests)
                {
                    collector.SendPieceRequest(peer, request.Index, request.Offset);
                    storage.Freeze(peer, request);
                }
            }
        }

        public void Piece(PeerHash peer, Piece piece)
        {
            lock (storage)
            {
                if (storage.IsComplete(piece.Index) == false)
                {
                    int block = piece.Offset / repository.Properties.BlockSize;

                    repository.SetPiece(piece.Index, block, piece.Data);
                    bool completed = storage.Complete(piece.Index, block);

                    if (completed && repository.Verify(piece.Index) == false)
                    {
                        storage.Invalidate(piece.Index);
                    }
                }
            }
        }
    }
}