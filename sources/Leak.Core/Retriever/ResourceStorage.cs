using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Omnibus;

namespace Leak.Core.Retriever
{
    public class ResourceStorage
    {
        private readonly OmnibusBitfield bitfields;
        private readonly ResourcePeerCollection peers;
        private readonly ResourceMetadataBookCollection metadata;

        public ResourceStorage(ResourceStorageConfiguration configuration)
        {
            this.bitfields = new OmnibusBitfield(configuration.ToOmnibus());
            this.peers = new ResourcePeerCollection();
            this.metadata = new ResourceMetadataBookCollection();
        }

        public ResourceStorage(ResourceStorage storage, ResourceStorageConfiguration configuration)
        {
            this.bitfields = new OmnibusBitfield(storage.bitfields, configuration.ToOmnibus());
            this.peers = storage.peers;
            this.metadata = storage.metadata;
        }

        public void AddBitfield(PeerHash peer, Bitfield bitfield)
        {
            bitfields.Add(peer, bitfield);
            peers.AddPeer(peer);
        }

        public void Complete(Bitfield bitfield)
        {
            bitfields.Complete(bitfield);
        }

        public bool Complete(PeerHash peer, ResourceBlock block)
        {
            peers.Increase(peer);

            return bitfields.Complete(block.ToOmnibus());
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
            return bitfields.Next(OmnibusStrategy.Sequential, peer, blocks).FromOmnibus();
        }

        public ResourceMetadataBlock[] ScheduleMetadata(PeerHash peer)
        {
            return metadata.Next(peer, 1);
        }

        public void Reserve(PeerHash peer, ResourceBlock request)
        {
            peers.Decrease(peer);
            peers.Decrease(bitfields.Reserve(peer, request.ToOmnibus()), 10);
        }

        public void Reserve(PeerHash peer, ResourceMetadataBlock request)
        {
            metadata.Add(peer, request);
        }
    }
}