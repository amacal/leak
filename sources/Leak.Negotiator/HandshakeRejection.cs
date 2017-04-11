namespace Leak.Peer.Negotiator
{
    public class HandshakeRejection
    {
        private readonly HandshakeMatch match;

        public HandshakeRejection(HandshakeMatch match)
        {
            this.match = match;
        }

        public HandshakeMatch Match
        {
            get { return match; }
        }
    }
}