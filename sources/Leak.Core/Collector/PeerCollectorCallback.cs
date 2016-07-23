using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Collector
{
    public interface PeerCollectorCallback
    {
        void OnConnected(PeerEndpoint endpoint);

        void OnDisconnected(PeerHash peer);

        void OnKeepAlive(PeerHash peer, KeepAliveMessage message);

        void OnChoke(PeerHash peer, ChokeMessage message);

        void OnUnchoke(PeerHash peer, UnchokeMessage message);

        void OnInterested(PeerHash peer, InterestedMessage message);

        void OnHave(PeerHash peer, HaveMessage message);

        void OnBitfield(PeerHash peer, BitfieldMessage message);

        void OnPiece(PeerHash peer, PieceMessage message);
    }
}