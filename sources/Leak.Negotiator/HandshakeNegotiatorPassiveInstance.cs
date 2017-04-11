using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
{
    public class HandshakeNegotiatorPassiveInstance : HandshakeNegotiatorPassiveContext
    {
        private readonly FileHashCollection hashes;
        private readonly PeerHash peer;
        private readonly HandshakeOptions options;

        public HandshakeNegotiatorPassiveInstance(PeerHash peer, FileHash hash, HandshakeOptions options)
        {
            this.peer = peer;
            this.options = options;
            this.hashes = new FileHashCollection(hash);
        }

        public HandshakeNegotiatorPassiveInstance(PeerHash peer, FileHashCollection hashes, HandshakeOptions options)
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