using Leak.Core.Common;
using Leak.Core.Messages;
using System.Collections.Generic;

namespace Leak.Core.Retriever
{
    public class ResourceStorage
    {
        private readonly ResourceBitfield bitfields;
        private readonly HashSet<PeerHash> peers;

        public ResourceStorage(ResourceConfiguration configuration)
        {
            this.bitfields = new ResourceBitfield(configuration);
            this.peers = new HashSet<PeerHash>();
        }

        public void AddBitfield(PeerHash peer, Bitfield bitfield)
        {
            bitfields.Add(peer, bitfield);
        }

        public void AddPeer(PeerHash peer)
        {
            peers.Add(peer);
        }

        public IEnumerable<PeerHash> GetPeers()
        {
            return peers;
        }

        public void Complete(Bitfield bitfield)
        {
            bitfields.Complete(bitfield);
        }

        public bool Complete(ResourcePieceRequest request)
        {
            return bitfields.Complete(request);
        }

        public void Invalidate(int piece)
        {
            bitfields.Invalidate(piece);
        }

        public bool IsComplete(int piece)
        {
            return bitfields.IsComplete(piece);
        }

        public ResourcePieceRequest[] Next(PeerHash peer)
        {
            return bitfields.Next(peer, 10);
        }

        public void Book(PeerHash peer, ResourcePieceRequest request)
        {
            bitfields.Book(peer, request);
        }
    }
}