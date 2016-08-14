using Leak.Core.Cando.Metadata;
using Leak.Core.Collector;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Client
{
    public class PeerClientToCollector : PeerCollectorCallbackBase
    {
        private readonly PeerClientCallback callback;
        private readonly PeerClientStorage storage;

        public PeerClientToCollector(PeerClientConfiguration configuration, PeerClientStorage storage)
        {
            this.callback = configuration.Callback;
            this.storage = storage;
        }

        public override void OnConnecting(PeerAddress peer)
        {
            callback.OnPeerConnecting(storage.GetHash(), peer);
        }

        public override void OnConnected(PeerCollectorConnected connected)
        {
            callback.OnPeerConnected(storage.GetHash(), new PeerClientConnected(connected));
        }

        public override void OnRejected(PeerAddress peer)
        {
            callback.OnPeerRejected(storage.GetHash(), peer);
        }

        public override void OnDisconnected(PeerHash peer)
        {
            FileHash hash = storage.GetHash(peer);

            storage.RemovePeer(peer);
            callback.OnPeerDisconnected(hash, peer);
        }

        public override void OnHandshake(PeerEndpoint endpoint)
        {
            if (storage.AddEndpoint(endpoint))
            {
                callback.OnPeerHandshake(endpoint.Hash, endpoint);
            }
        }

        public override void OnIncoming(PeerHash peer, PeerCollectorMessage message)
        {
            FileHash hash = storage.GetHash(peer);
            PeerClientMessage payload = new PeerClientMessage(message);

            callback.OnPeerIncomingMessage(hash, peer, payload);
        }

        public override void OnOutgoing(PeerHash peer, PeerCollectorMessage message)
        {
            FileHash hash = storage.GetHash(peer);
            PeerClientMessage payload = new PeerClientMessage(message);

            callback.OnPeerOutgoingMessage(hash, peer, payload);
        }

        public override void OnBitfield(PeerHash peer, BitfieldMessage message)
        {
            int size = message.Size;

            if (storage.HasMetainfo(peer))
            {
                size = storage.GetMetainfo(peer).Pieces.Length;
            }

            FileHash hash = storage.GetHash(peer);
            Bitfield bitfield = new Bitfield(size);

            for (int i = 0; i < size; i++)
            {
                bitfield[i] = message[i];
            }

            callback.OnPeerBitfield(hash, peer, bitfield);
            storage.GetRetriever(peer).SetBitfield(peer, bitfield);
        }

        public override void OnChoke(PeerHash peer, ChokeMessage message)
        {
            FileHash hash = storage.GetHash(peer);
            callback.OnPeerChoked(hash, peer);
        }

        public override void OnUnchoke(PeerHash peer, UnchokeMessage message)
        {
            FileHash hash = storage.GetHash(peer);
            callback.OnPeerUnchoked(hash, peer);
        }

        public override void OnPiece(PeerHash peer, PieceMessage message)
        {
            if (storage.HasMetainfo(peer))
            {
                FileHash hash = storage.GetHash(peer);
                Piece piece = new Piece(message.Piece, message.Offset, message.Size, message.Data);

                callback.OnBlockReceived(hash, peer, piece);
                storage.GetRetriever(peer).AddPiece(peer, piece);
            }
        }

        public override void OnMetadataReceived(PeerHash peer, MetadataData metadata)
        {
            if (storage.HasMetainfo(peer) == false)
            {
                FileHash hash = storage.GetHash(peer);

                callback.OnMetadataReceived(hash, peer, metadata);
                storage.GetRetriever(peer).AddMetadata(peer, metadata);
            }
        }
    }
}