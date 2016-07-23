using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Metadata;

namespace Leak.Core.Client
{
    public abstract class PeerClientCallbackBase : PeerClientCallback
    {
        public virtual void OnInitialized(Metainfo metainfo, PeerClientMetainfoSummary summary)
        {
        }

        public virtual void OnStarted(Metainfo metainfo)
        {
        }

        public virtual void OnCompleted(Metainfo metainfo)
        {
        }

        public virtual void OnPeerConnecting(Metainfo metainfo, string endpoint)
        {
        }

        public virtual void OnPeerConnected(Metainfo metainfo, PeerEndpoint endpoint)
        {
        }

        public virtual void OnPeerDisconnected(Metainfo metainfo, PeerHash peer)
        {
        }

        public virtual void OnPeerBitfield(Metainfo metainfo, PeerHash peer, Bitfield bitfield)
        {
        }

        public virtual void OnPeerChoked(Metainfo metainfo, PeerHash peer)
        {
        }

        public virtual void OnPeerUnchoked(Metainfo metainfo, PeerHash peer)
        {
        }

        public virtual void OnBlockReceived(Metainfo metainfo, PeerHash peer, Piece piece)
        {
        }

        public virtual void OnPieceVerified(Metainfo metainfo, PeerClientPieceVerification verification)
        {
        }
    }
}