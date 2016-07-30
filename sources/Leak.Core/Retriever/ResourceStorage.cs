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
        private readonly ResourceMetadataBookCollection metadata;

        public ResourceStorage(ResourceStorageConfiguration configuration)
        {
            this.bitfields = new ResourceBitfield(configuration);
            this.peers = new ResourcePeerCollection();
            this.metadata = new ResourceMetadataBookCollection();
        }

        public ResourceStorage(ResourceStorage storage, ResourceStorageConfiguration configuration)
        {
            this.bitfields = new ResourceBitfield(storage.bitfields, configuration);
            this.peers = storage.peers;
            this.metadata = storage.metadata;
        }

        public void AddBitfield(PeerHash peer, Bitfield bitfield)
        {
            peers.AddPeer(peer);
            bitfields.Add(peer, bitfield);
        }

        public void Choke(PeerHash peer)
        {
            peers.AddPeer(peer);
            peers.Choke(peer);
        }

        public void Unchoke(PeerHash peer)
        {
            peers.AddPeer(peer);
            peers.Unchoke(peer);
        }

        public IEnumerable<ResourcePeer> GetPeers(ResourcePeerOperation operation)
        {
            Func<ResourcePeer, bool> predicate = x => true;

            if (operation == ResourcePeerOperation.Request)
            {
                predicate = x => x.IsUnchoke();
            }

            if (operation == ResourcePeerOperation.Metadata)
            {
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

        public void Complete(int size)
        {
            metadata.Complete(size);
        }

        public void Complete(PeerHash peer, ResourceMetadataBlock block, int size)
        {
            metadata.Complete(block, size);
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

        public bool IsMetadataComplete()
        {
            return metadata.IsComplete();
        }

        public ResourceBlock[] Next(PeerHash peer, int blocks)
        {
            return bitfields.Next(peer, blocks);
        }

        public ResourceMetadataBlock[] ScheduleMetadata(PeerHash peer)
        {
            return metadata.Next(peer, 1);
        }

        public void Reserve(PeerHash peer, ResourceBlock request)
        {
            peers.Decrease(peer);
            peers.Decrease(bitfields.Reserve(peer, request), 10);
        }

        public void Reserve(PeerHash peer, ResourceMetadataBlock request)
        {
            metadata.Add(peer, request);
        }
    }
}