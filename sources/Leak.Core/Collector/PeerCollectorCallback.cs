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

        void OnIncoming(PeerHash peer, PeerCollectorMessage message);

        void OnOutgoing(PeerHash peer, PeerCollectorMessage message);

        void OnChoke(PeerHash peer, ChokeMessage message);

        void OnUnchoke(PeerHash peer, UnchokeMessage message);

        void OnInterested(PeerHash peer, InterestedMessage message);

        void OnHave(PeerHash peer, HaveMessage message);

        void OnBitfield(PeerHash peer, BitfieldMessage message);

        void OnPiece(PeerHash peer, PieceMessage message);

        void OnExtended(PeerHash peer, ExtendedIncomingMessage message);
    }
}