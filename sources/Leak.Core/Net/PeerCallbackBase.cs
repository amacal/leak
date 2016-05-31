using Leak.Core.IO;

namespace Leak.Core.Net
{
    public abstract class PeerCallbackBase : PeerCallback
    {
        private readonly PeerCollection peers;
        private readonly TorrentPieceCollection pieces;

        protected PeerCallbackBase(TorrentPieceCollection pieces)
        {
            this.peers = new PeerCollection();
            this.pieces = pieces;
        }

        public PeerCollection Peers
        {
            get { return peers; }
        }

        public override void OnAttached(PeerChannel channel)
        {
        }

        public override void OnTerminate(PeerChannel channel)
        {
        }

        public override void OnKeepAlive(PeerChannel channel)
        {
        }

        public override void OnUnchoke(PeerChannel channel, PeerUnchoke message)
        {
            lock (this)
            {
                peers.ByChannel(channel).Remote.Unchoke();
            }
        }

        public override void OnInterested(PeerChannel channel, PeerInterested message)
        {
        }

        public override void OnHave(PeerChannel channel, PeerHave message)
        {
            lock (this)
            {
                peers.ByChannel(channel).Remote.Bitfield.Apply(message);
            }
        }

        public override void OnBitfield(PeerChannel channel, PeerBitfield message)
        {
            lock (this)
            {
                peers.ByChannel(channel).Remote.Bitfield.Apply(message);
            }
        }

        public override void OnPiece(PeerChannel channel, PeerPiece message)
        {
        }
    }
}