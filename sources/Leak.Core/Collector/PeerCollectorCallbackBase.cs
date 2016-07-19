using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Collector
{
    public abstract class PeerCollectorCallbackBase : PeerCollectorCallback
    {
        public virtual void OnConnected(PeerHash peer, FileHash hash)
        {
        }

        public virtual void OnDisconnected(PeerHash peer)
        {
        }

        public virtual void OnKeepAlive(PeerHash peer, KeepAliveMessage message)
        {
        }

        public virtual void OnUnchoke(PeerHash peer, UnchokeMessage message)
        {
        }

        public virtual void OnInterested(PeerHash peer, InterestedMessage message)
        {
        }

        public virtual void OnBitfield(PeerHash peer, BitfieldMessage message)
        {
        }

        public virtual void OnPiece(PeerHash peer, PieceMessage message)
        {
        }
    }
}