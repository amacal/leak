using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Collector
{
    public abstract class PeerCollectorCallbackBase : PeerCollectorCallback
    {
        public virtual void OnConnected(PeerEndpoint endpoint)
        {
        }

        public virtual void OnDisconnected(PeerHash peer)
        {
        }

        public virtual void OnKeepAlive(PeerHash peer, KeepAliveMessage message)
        {
        }

        public virtual void OnChoke(PeerHash peer, ChokeMessage message)
        {
        }

        public virtual void OnUnchoke(PeerHash peer, UnchokeMessage message)
        {
        }

        public virtual void OnInterested(PeerHash peer, InterestedMessage message)
        {
        }

        public virtual void OnHave(PeerHash peer, HaveMessage message)
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