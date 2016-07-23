using Leak.Core.Collector;
using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Metadata;
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
            Metainfo metainfo = storage.GetMetainfo(endpoint.Hash);

            storage.AddPeer(endpoint.Hash, endpoint.Peer);
            callback.OnPeerConnected(metainfo, endpoint);
        }

        public override void OnDisconnected(PeerHash peer)
        {
            Metainfo metainfo = storage.GetMetainfo(peer);

            storage.RemovePeer(peer);
            callback.OnPeerDisconnected(metainfo, peer);
        }

        public override void OnBitfield(PeerHash peer, BitfieldMessage message)
        {
            Metainfo metainfo = storage.GetMetainfo(peer);
            Bitfield bitfield = new Bitfield(metainfo.Pieces.Length);

            for (int i = 0; i < metainfo.Pieces.Length; i++)
            {
                bitfield[i] = message[i];
            }

            callback.OnPeerBitfield(metainfo, peer, bitfield);
            storage.GetRetriever(peer).Start(peer, bitfield);
        }

        public override void OnChoke(PeerHash peer, ChokeMessage message)
        {
            Metainfo metainfo = storage.GetMetainfo(peer);
            ResourceDirection direction = ResourceDirection.Local;

            callback.OnPeerChoked(metainfo, peer);
            storage.GetRetriever(peer).SetChoked(peer, direction);
        }

        public override void OnUnchoke(PeerHash peer, UnchokeMessage message)
        {
            Metainfo metainfo = storage.GetMetainfo(peer);
            ResourceDirection direction = ResourceDirection.Local;

            callback.OnPeerUnchoked(metainfo, peer);
            storage.GetRetriever(peer).SetUnchoked(peer, direction);
        }

        public override void OnPiece(PeerHash peer, PieceMessage message)
        {
            Metainfo metainfo = storage.GetMetainfo(peer);
            Piece piece = new Piece(message.Piece, message.Offset, message.Size, message.Data);

            callback.OnBlockReceived(metainfo, peer, piece);
            storage.GetRetriever(peer).AddPiece(peer, piece);
        }
    }
}