using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Collector
{
    public interface PeerCollectorCallback
    {
        void OnConnecting(PeerAddress peer);

        void OnConnected(PeerCollectorConnected connected);

        void OnDisconnected(PeerHash peer);

        void OnRejected(PeerAddress peer);

        void OnHandshake(PeerEndpoint endpoint);

        void OnIncoming(PeerEndpoint endpoint, PeerCollectorMessage message);

        void OnOutgoing(PeerEndpoint endpoint, PeerCollectorMessage message);

        void OnChoke(PeerEndpoint endpoint, ChokeMessage message);

        void OnUnchoke(PeerEndpoint endpoint, UnchokeMessage message);

        void OnInterested(PeerEndpoint endpoint, InterestedMessage message);

        void OnHave(PeerEndpoint endpoint, HaveMessage message);

        void OnBitfield(PeerEndpoint endpoint, BitfieldMessage message);

        void OnPiece(PeerEndpoint endpoint, PieceMessage message);

        void OnMetadataSize(PeerHash peer, MetadataSize size);

        void OnMetadataReceived(PeerHash peer, MetadataData metadata);
    }
}