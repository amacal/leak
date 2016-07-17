using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Collector
{
    public interface PeerCollectorCallback
    {
        void OnConnected(PeerHash hash);

        void OnDisconnected(PeerHash hash);

        void OnKeepAlive(PeerHash hash, KeepAliveMessage message);

        void OnUnchoke(PeerHash hash, UnchokeMessage message);

        void OnInterested(PeerHash hash, InterestedMessage message);

        void OnBitfield(PeerHash hash, BitfieldMessage message);

        void OnPiece(PeerHash hash, PieceMessage message);
    }
}