using Leak.Core.Common;
using Leak.Core.Extensions.Metadata;
using Leak.Core.Messages;

namespace Leak.Core.Client
{
    public abstract class PeerClientCallbackBase : PeerClientCallback
    {
        public virtual void OnInitialized(FileHash hash, PeerClientMetainfoSummary summary)
        {
        }

        public virtual void OnStarted(FileHash hash)
        {
        }

        public virtual void OnCompleted(FileHash hash)
        {
        }

        public virtual void OnPeerConnecting(FileHash hash, string endpoint)
        {
        }

        public virtual void OnPeerConnected(FileHash hash, PeerEndpoint endpoint)
        {
        }

        public virtual void OnPeerDisconnected(FileHash hash, PeerHash peer)
        {
        }

        public virtual void OnPeerBitfield(FileHash hash, PeerHash peer, Bitfield bitfield)
        {
        }

        public virtual void OnPeerChoked(FileHash hash, PeerHash peer)
        {
        }

        public virtual void OnPeerUnchoked(FileHash hash, PeerHash peer)
        {
        }

        public virtual void OnBlockReceived(FileHash hash, PeerHash peer, Piece piece)
        {
        }

        public virtual void OnPieceVerified(FileHash hash, PeerClientPieceVerification verification)
        {
        }

        public virtual void OnMetadataReceived(FileHash hash, PeerHash peer, MetadataData data)
        {
        }
    }
}