using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Collector
{
    public abstract class PeerCollectorCallbackBase : PeerCollectorCallback
    {
        public virtual void OnConnected(PeerHash hash)
        {
        }

        public virtual void OnDisconnected(PeerHash hash)
        {
        }

        public virtual void OnKeepAlive(PeerHash hash, KeepAliveMessage message)
        {
        }

        public virtual void OnUnchoke(PeerHash hash, UnchokeMessage message)
        {
        }

        public virtual void OnInterested(PeerHash hash, InterestedMessage message)
        {
        }

        public virtual void OnBitfield(PeerHash hash, BitfieldMessage message)
        {
        }

        public virtual void OnPiece(PeerHash hash, PieceMessage message)
        {
        }
    }
}