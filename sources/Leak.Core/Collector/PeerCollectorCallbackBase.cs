using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Collector
{
    public abstract class PeerCollectorCallbackBase : PeerCollectorCallback
    {
        public virtual void OnConnecting(PeerAddress peer)
        {
        }

        public virtual void OnConnected(PeerCollectorConnected connected)
        {
        }

        public virtual void OnRejected(PeerAddress peer)
        {
        }

        public virtual void OnDisconnected(PeerHash peer)
        {
        }

        public virtual void OnHandshake(PeerEndpoint endpoint)
        {
        }

        public virtual void OnIncoming(PeerEndpoint endpoint, PeerCollectorMessage message)
        {
        }

        public virtual void OnOutgoing(PeerEndpoint endpoint, PeerCollectorMessage message)
        {
        }

        public virtual void OnChoke(PeerEndpoint endpoint, ChokeMessage message)
        {
        }

        public virtual void OnUnchoke(PeerEndpoint endpoint, UnchokeMessage message)
        {
        }

        public virtual void OnInterested(PeerEndpoint endpoint, InterestedMessage message)
        {
        }

        public virtual void OnHave(PeerEndpoint endpoint, HaveMessage message)
        {
        }

        public virtual void OnBitfield(PeerEndpoint endpoint, BitfieldMessage message)
        {
        }

        public virtual void OnPiece(PeerEndpoint endpoint, PieceMessage message)
        {
        }

        public virtual void OnMetadataSize(PeerHash peer, MetadataSize size)
        {
        }

        public virtual void OnMetadataReceived(PeerHash peer, MetadataData metadata)
        {
        }
    }
}