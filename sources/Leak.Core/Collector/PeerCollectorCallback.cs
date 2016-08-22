using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Collector
{
    public interface PeerCollectorCallback
    {
        void OnConnecting(FileHash hash, PeerAddress peer);

        void OnConnected(PeerCollectorConnected connected);

        void OnDisconnected(PeerSession session);

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

        void OnMetadataSize(PeerSession session, MetadataSize size);

        void OnMetadataReceived(PeerSession session, MetadataData metadata);
    }
}