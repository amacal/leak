using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Metadata;

namespace Leak.Core.Client
{
    public interface PeerClientCallback
    {
        void OnInitialized(Metainfo metainfo, PeerClientMetainfoSummary summary);

        void OnStarted(Metainfo metainfo);

        void OnCompleted(Metainfo metainfo);

        void OnPeerConnecting(Metainfo metainfo, string endpoint);

        void OnPeerConnected(Metainfo metainfo, PeerEndpoint endpoint);

        void OnPeerDisconnected(Metainfo metainfo, PeerHash peer);

        void OnPeerBitfield(Metainfo metainfo, PeerHash peer, Bitfield bitfield);

        void OnPeerChoked(Metainfo metainfo, PeerHash peer);

        void OnPeerUnchoked(Metainfo metainfo, PeerHash peer);

        void OnBlockReceived(Metainfo metainfo, PeerHash peer, Piece piece);

        void OnPieceVerified(Metainfo metainfo, PeerClientPieceVerification verification);
    }
}