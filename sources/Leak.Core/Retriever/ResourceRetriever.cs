using Leak.Core.Common;
using Leak.Core.Extensions;
using Leak.Core.Extensions.Metadata;
using Leak.Core.Messages;
using Leak.Core.Repository;

namespace Leak.Core.Retriever
{
    public interface ResourceRetriever
    {
        ResourceRetriever WithBitfield(Bitfield bitfield);

        ResourceRetriever WithRepository(ResourceRepository repository);

        void SetExtensions(PeerHash peer, ExtenderHandshake handshake);

        void SetBitfield(PeerHash peer, Bitfield bitfield);

        void SetChoked(PeerHash peer, ResourceDirection direction);

        void SetUnchoked(PeerHash peer, ResourceDirection direction);

        void AddPiece(PeerHash peer, Piece piece);

        void AddMetadata(PeerHash peer, MetadataData data);
    }
}