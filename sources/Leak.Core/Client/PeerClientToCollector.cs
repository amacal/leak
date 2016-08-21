using Leak.Core.Cando.Metadata;
using Leak.Core.Collector;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Client
{
    public class PeerClientToCollector : PeerCollectorCallbackBase
    {
        private readonly PeerClientContext context;

        public PeerClientToCollector(PeerClientContext context)
        {
            this.context = context;
        }

        public override void OnConnecting(PeerAddress peer)
        {
        }

        public override void OnConnected(PeerCollectorConnected connected)
        {
        }

        public override void OnRejected(PeerAddress peer)
        {
        }

        public override void OnDisconnected(PeerHash peer)
        {
        }

        public override void OnHandshake(PeerEndpoint endpoint)
        {
            context.Callback.OnPeerHandshake(endpoint.Hash, endpoint);
        }

        public override void OnIncoming(PeerEndpoint endpoint, PeerCollectorMessage message)
        {
        }

        public override void OnOutgoing(PeerEndpoint endpoint, PeerCollectorMessage message)
        {
        }

        public override void OnBitfield(PeerEndpoint endpoint, BitfieldMessage message)
        {
            context.Callback.OnPeerBitfield(endpoint.Hash, endpoint.Peer, message.ToBitfield());

            context.Tasking.Handle(with =>
            {
                with.OnPeerBitfield(endpoint.Peer, message.ToBitfield());
            });
        }

        public override void OnChoke(PeerEndpoint endpoint, ChokeMessage message)
        {
            context.Callback.OnPeerChoked(endpoint.Hash, endpoint.Peer);
        }

        public override void OnUnchoke(PeerEndpoint endpoint, UnchokeMessage message)
        {
            context.Callback.OnPeerUnchoked(endpoint.Hash, endpoint.Peer);
        }

        public override void OnPiece(PeerEndpoint endpoint, PieceMessage message)
        {
            context.Tasking.Handle(with =>
            {
                with.OnPeerPiece(endpoint.Peer, message.ToPiece());
            });
        }

        public override void OnMetadataSize(PeerHash peer, MetadataSize size)
        {
            context.Tasking.Handle(with =>
            {
                with.OnMetadataSize(peer, size);
            });
        }

        public override void OnMetadataReceived(PeerHash peer, MetadataData metadata)
        {
            context.Tasking.Handle(with =>
            {
                with.OnMetadataData(peer, metadata);
            });
        }
    }
}