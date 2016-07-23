using Leak.Core.Common;
using Leak.Core.Messages;
using System.Collections.Generic;

namespace Leak.Core.Retriever
{
    public class ResourceStorage
    {
        private readonly ResourceBitfield bitfields;
        private readonly HashSet<PeerHash> peers;

        public ResourceStorage(ResourceStorageConfiguration configuration)
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

        public bool Complete(ResourceBlock block)
        {
            return bitfields.Complete(block);
        }

        public void Invalidate(int piece)
        {
            bitfields.Invalidate(piece);
        }

        public bool IsComplete()
        {
            return bitfields.IsComplete();
        }

        public bool IsComplete(int piece)
        {
            return bitfields.IsComplete(piece);
        }

        public ResourceBlock[] Next(PeerHash peer)
        {
            return bitfields.Next(peer, 16);
        }

        public void Book(PeerHash peer, ResourceBlock request)
        {
            bitfields.Book(peer, request);
        }
    }
}