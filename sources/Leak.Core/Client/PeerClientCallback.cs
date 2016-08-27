using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Client
{
    public interface PeerClientCallback
    {
        /// <summary>
        /// Called when the resource is scheduled.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnFileScheduled(FileHash hash);

        /// <summary>
        /// Called when the resource is discovered.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnFileDiscovered(FileHash hash);

        /// <summary>
        /// Called when the resource is initialized.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="summary">Describes the affected resource.</param>
        void OnFileInitialized(FileHash hash, PeerClientMetainfo summary);

        /// <summary>
        /// Called when the downloading started.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnFileStarted(FileHash hash);

        /// <summary>
        /// Called when the downloading completed.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnFileCompleted(FileHash hash);

        void OnPeerConnecting(FileHash hash, PeerAddress peer);

        void OnPeerConnected(FileHash hash, PeerClientConnected connected);

        void OnPeerRejected(FileHash hash, PeerAddress peer);

        void OnPeerDisconnected(FileHash hash, PeerHash peer);

        void OnPeerHandshake(FileHash hash, PeerEndpoint endpoint);

        void OnPeerIncomingMessage(FileHash hash, PeerHash peer, PeerClientMessage message);

        void OnPeerOutgoingMessage(FileHash hash, PeerHash peer, PeerClientMessage message);

        void OnPeerBitfield(FileHash hash, PeerHash peer, Bitfield bitfield);

        void OnPeerChoked(FileHash hash, PeerHash peer);

        void OnPeerUnchoked(FileHash hash, PeerHash peer);

        void OnBlockReceived(FileHash hash, PeerHash peer, Piece piece);

        void OnPieceVerified(FileHash hash, PeerClientPieceVerification verification);

        void OnMetadataReceived(FileHash hash, PeerHash peer, MetadataData data);
    }
}