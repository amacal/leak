namespace Leak.Core.Negotiator
{
    public class Handshake
    {
        private readonly HandshakePeer local;
        private readonly HandshakePeer remote;
        private readonly HandshakeHash hash;

        public Handshake(HandshakePeer local, HandshakePeer remote, HandshakeHash hash)
        {
            this.local = local;
            this.remote = remote;
            this.hash = hash;
        }

        public HandshakePeer Local
        {
            get { return local; }
        }

        public HandshakePeer Remote
        {
            get { return remote; }
        }

        public HandshakeHash Hash
        {
            get { return hash; }
        }
    }
}