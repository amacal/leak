using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Metadata;

namespace Leak.Core.Client
{
    public abstract class PeerClientCallbackBase : PeerClientCallback
    {
        public virtual void OnInitialized(Metainfo metainfo, MetainfoSummary summary)
        {
        }

        public virtual void OnStarted(Metainfo metainfo)
        {
        }

        public virtual void OnCompleted(Metainfo metainfo)
        {
        }

        public virtual void OnPeerConnected(Metainfo metainfo, PeerHash peer)
        {
        }

        public virtual void OnPeerBitfield(Metainfo metainfo, PeerHash peer, Bitfield bitfield)
        {
        }

        public virtual void OnPeerUnchoked(Metainfo metainfo, PeerHash peer)
        {
        }

        public virtual void OnPieceReceived(Metainfo metainfo, PeerHash peer, Piece piece)
        {
        }
    }
}