using Leak.Core.Collector;
using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Retriever;

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

        public override void OnConnected(PeerEndpoint endpoint)
        {
            if (storage.AddEndpoint(endpoint))
            {
                callback.OnPeerConnected(endpoint.Hash, endpoint);
            }
        }

        public override void OnDisconnected(PeerHash peer)
        {
            FileHash hash = storage.GetHash(peer);

            storage.RemovePeer(peer);
            callback.OnPeerDisconnected(hash, peer);
        }

        public override void OnBitfield(PeerHash peer, BitfieldMessage message)
        {
            int size = message.Length;

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
            ResourceDirection direction = ResourceDirection.Local;

            callback.OnPeerChoked(hash, peer);
            storage.GetRetriever(peer).SetChoked(peer, direction);
        }

        public override void OnUnchoke(PeerHash peer, UnchokeMessage message)
        {
            FileHash hash = storage.GetHash(peer);
            ResourceDirection direction = ResourceDirection.Local;

            callback.OnPeerUnchoked(hash, peer);
            storage.GetRetriever(peer).SetUnchoked(peer, direction);
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

        public override void OnExtended(PeerHash peer, ExtendedIncomingMessage message)
        {
            storage.GetExtender(peer).Handle(peer, message);
        }
    }
}