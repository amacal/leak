using Leak.Core.Common;
using Leak.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Retriever
{
    public class ResourceStorage
    {
        private readonly ResourceBitfield bitfields;
        private readonly ResourcePeerCollection peers;

        public ResourceStorage(ResourceStorageConfiguration configuration)
        {
            this.bitfields = new ResourceBitfield(configuration);
            this.peers = new ResourcePeerCollection();
        }

        public void AddBitfield(PeerHash peer, Bitfield bitfield)
        {
            bitfields.Add(peer, bitfield);
        }

        public void AddPeer(PeerHash peer)
        {
            peers.AddPeer(peer);
        }

        public void Choke(PeerHash peer)
        {
            peers.Choke(peer);
        }

        public void Unchoke(PeerHash peer)
        {
            peers.Unchoke(peer);
        }

        public IEnumerable<ResourcePeer> GetPeers(ResourcePeerOperation operation)
        {
            Func<ResourcePeer, bool> predicate = x => true;

            if (operation == ResourcePeerOperation.Request)
            {
                predicate = x => x.IsUnchoke();
            }

            return peers.Where(predicate).OrderByDescending(x => x.Rank);
        }

        public void Complete(Bitfield bitfield)
        {
            bitfields.Complete(bitfield);
        }

        public bool Complete(PeerHash peer, ResourceBlock block)
        {
            peers.Increase(peer);

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
            peers.Decrease(peer);
            bitfields.Book(peer, request);
        }
    }
}