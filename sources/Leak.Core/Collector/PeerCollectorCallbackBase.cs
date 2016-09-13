using Leak.Core.Cando.Metadata;
using Leak.Core.Cando.PeerExchange;
using Leak.Core.Collector.Events;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Collector
{
    public abstract class PeerCollectorCallbackBase : PeerCollectorCallback
    {
        public virtual void OnListenerStarted(PeerCollectorListenerStarted started)
        {
        }

        public virtual void OnConnectingTo(FileHash hash, PeerAddress peer)
        {
        }

        public virtual void OnConnectingFrom(PeerAddress peer)
        {
        }

        public virtual void OnConnectedTo(FileHash hash, PeerCollectorConnected connected)
        {
        }

        public virtual void OnConnectedFrom(PeerCollectorConnected connected)
        {
        }

        public virtual void OnRejected(PeerAddress peer)
        {
        }

        public virtual void OnDisconnected(PeerSession session)
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

        public virtual void OnBitfield(PeerSession session, Bitfield bitfield)
        {
        }

        public virtual void OnPiece(PeerEndpoint endpoint, PieceMessage message)
        {
        }

        public virtual void OnMetadataSize(PeerSession session, MetadataSize size)
        {
        }

        public virtual void OnMetadataReceived(PeerSession session, MetadataData data)
        {
        }

        public virtual void OnPeerExchanged(PeerSession session, PeerExchangeData data)
        {
        }
    }
}