namespace Leak.Core.Net
{
    public abstract class PeerCallback
    {
        public abstract void OnAttached(PeerChannel channel);

        public abstract void OnTerminate(PeerChannel channel);

        public abstract void OnKeepAlive(PeerChannel channel);

        public abstract void OnUnchoke(PeerChannel channel, PeerUnchoke message);

        public abstract void OnHave(PeerChannel channel, PeerHave message);

        public abstract void OnBitfield(PeerChannel channel, PeerBitfield message);

        public abstract void OnPiece(PeerChannel channel, PeerPiece message);
    }
}