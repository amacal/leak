using Leak.Core.Common;
using Leak.Core.Extensions.Metadata;
using Leak.Core.Messages;

namespace Leak.Core.Client
{
    public interface PeerClientCallback
    {
        void OnInitialized(FileHash hash, PeerClientMetainfoSummary summary);

        void OnStarted(FileHash hash);

        void OnCompleted(FileHash hash);

        void OnPeerConnecting(FileHash hash, string endpoint);

        void OnPeerConnected(FileHash hash, PeerEndpoint endpoint);

        void OnPeerDisconnected(FileHash hash, PeerHash peer);

        void OnPeerBitfield(FileHash hash, PeerHash peer, Bitfield bitfield);

        void OnPeerChoked(FileHash hash, PeerHash peer);

        void OnPeerUnchoked(FileHash hash, PeerHash peer);

        void OnBlockReceived(FileHash hash, PeerHash peer, Piece piece);

        void OnPieceVerified(FileHash hash, PeerClientPieceVerification verification);

        void OnMetadataReceived(FileHash hash, PeerHash peer, MetadataData data);
    }
}