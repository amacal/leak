using Leak.Common;

namespace Leak.Negotiator
{
    public class HandshakeNegotiatorPassiveInstance : HandshakeNegotiatorPassiveContext
    {
        private readonly FileHashCollection hashes;
        private readonly PeerHash peer;
        private readonly HandshakeOptions options;

        public HandshakeNegotiatorPassiveInstance(FileHashCollection hashes, PeerHash peer, HandshakeOptions options)
        {
            this.hashes = hashes;
            this.peer = peer;
            this.options = options;
        }

        public FileHashCollection Hashes
        {
            get { return hashes; }
        }

        public PeerHash Peer
        {
            get { return peer; }
        }

        public HandshakeOptions Options
        {
            get { return options; }
        }

        public void OnRejected(HandshakeRejection rejection)
        {
        }

        public void OnHandshake(NetworkConnection negotiated, Handshake handshake)
        {
        }

        public void OnDisconnected()
        {
        }
    }
}
