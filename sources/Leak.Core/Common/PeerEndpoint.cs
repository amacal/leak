namespace Leak.Core.Common
{
    public class PeerEndpoint
    {
        private readonly PeerSession session;
        private readonly PeerAddress remote;
        private readonly PeerDirection direction;

        public PeerEndpoint(PeerSession session, PeerAddress remote, PeerDirection direction)
        {
            this.session = session;
            this.remote = remote;
            this.direction = direction;
        }

        public PeerAddress Remote
        {
            get { return remote; }
        }

        public PeerDirection Direction
        {
            get { return direction; }
        }

        public PeerSession Session
        {
            get { return session; }
        }
    }
}