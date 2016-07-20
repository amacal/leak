using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Metadata;

namespace Leak.Core.Client
{
    public interface PeerClientCallback
    {
        void OnStarted(Metainfo metainfo);

        void OnCompleted(Metainfo metainfo);

        void OnPeerConnected(Metainfo metainfo, PeerHash peer);

        void OnPeerBitfield(Metainfo metainfo, PeerHash peer, Bitfield bitfield);

        void OnPeerUnchoked(Metainfo metainfo, PeerHash peer);

        void OnPieceReceived(Metainfo metainfo, PeerHash peer, Piece piece);
    }
}