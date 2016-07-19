using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Collector
{
    public interface PeerCollectorCallback
    {
        void OnConnected(PeerHash peer, FileHash hash);

        void OnDisconnected(PeerHash peer);

        void OnKeepAlive(PeerHash peer, KeepAliveMessage message);

        void OnUnchoke(PeerHash peer, UnchokeMessage message);

        void OnInterested(PeerHash peer, InterestedMessage message);

        void OnBitfield(PeerHash peer, BitfieldMessage message);

        void OnPiece(PeerHash peer, PieceMessage message);
    }
}