using Leak.Core.Cando.Metadata;
using Leak.Core.Cando.PeerExchange;
using Leak.Core.Collector.Events;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Collector
{
    public interface PeerCollectorCallback
    {
        /// <summary>
        /// Called when the new outgoing connection is being established.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="peer">The remote peer address.</param>
        void OnConnectingTo(FileHash hash, PeerAddress peer);

        /// <summary>
        /// Called when the new incoming connection is being established.
        /// </summary>
        /// <param name="peer">The remote peer address.</param>
        void OnConnectingFrom(PeerAddress peer);

        /// <summary>
        /// Called when the new outgoing connection was successfully
        /// established with the remote peer.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="connected">Describes the current state.</param>
        void OnConnectedTo(FileHash hash, PeerCollectorConnected connected);

        /// <summary>
        /// Called when the new incoming connection was successfully
        /// established with the remote peer.
        /// </summary>
        /// <param name="connected">Describes the current state.</param>
        void OnConnectedFrom(PeerCollectorConnected connected);

        void OnDisconnected(PeerSession session);

        void OnRejected(PeerAddress peer);

        void OnHandshake(PeerEndpoint endpoint);

        void OnIncoming(PeerEndpoint endpoint, PeerCollectorMessage message);

        void OnOutgoing(PeerEndpoint endpoint, PeerCollectorMessage message);

        void OnChoke(PeerEndpoint endpoint, ChokeMessage message);

        void OnUnchoke(PeerEndpoint endpoint, UnchokeMessage message);

        void OnInterested(PeerEndpoint endpoint, InterestedMessage message);

        void OnBitfield(PeerSession session, Bitfield bitfield);

        void OnPiece(PeerEndpoint endpoint, PieceMessage message);

        void OnMetadataSize(PeerSession session, MetadataSize size);

        void OnMetadataReceived(PeerSession session, MetadataData data);

        void OnPeerExchanged(PeerSession session, PeerExchangeData data);
    }
}