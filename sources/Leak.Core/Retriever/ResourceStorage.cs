using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Retriever
{
    public class ResourceStorage
    {
        private readonly ResourceBitfield bitfields;

        public ResourceStorage(ResourceConfiguration configuration)
        {
            this.bitfields = new ResourceBitfield(configuration);
        }

        public void AddBitfield(PeerHash peer, Bitfield bitfield)
        {
            bitfields.Add(peer, bitfield);
        }

        public bool Complete(int piece, int offset)
        {
            return bitfields.Complete(piece, offset / 32678);
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

        public void Freeze(PeerHash peer, ResourcePieceRequest request)
        {
            bitfields.Freeze(peer, request);
        }
    }
}