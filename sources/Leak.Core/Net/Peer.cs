using Leak.Core.IO;

namespace Leak.Core.Net
{
    public class Peer
    {
        private readonly PeerChannel channel;
        private readonly TorrentPieceCollection pieces;
        private readonly PeerHandshakePayload handshake;

        private PeerEndpoint local;
        private PeerEndpoint remote;

        public Peer(PeerChannel channel, TorrentPieceCollection pieces, PeerHandshakePayload handshake)
        {
            this.channel = channel;
            this.pieces = pieces;
            this.handshake = handshake;

            this.local = new PeerEndpoint(pieces);
            this.remote = new PeerEndpoint(pieces);
        }

        public PeerChannel Channel
        {
            get { return channel; }
        }

        public PeerEndpoint Local
        {
            get { return local; }
        }

        public PeerEndpoint Remote
        {
            get { return remote; }
        }

        public TorrentPieceView Pending
        {
            get { return new TorrentPieceView(pieces, piece => local.Bitfield[piece.Index] == false); }
        }

        public TorrentPieceView Available
        {
            get { return new TorrentPieceView(pieces, piece => remote.Bitfield[piece.Index] == true); }
        }
    }
}