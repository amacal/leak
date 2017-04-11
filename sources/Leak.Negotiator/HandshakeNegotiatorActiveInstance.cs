using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
{
    public class HandshakeNegotiatorActiveInstance : HandshakeNegotiatorActiveContext
    {
        private readonly FileHash hash;
        private readonly PeerHash peer;
        private readonly HandshakeOptions options;

        public HandshakeNegotiatorActiveInstance(PeerHash peer, FileHash hash, HandshakeOptions options)
        {
            this.hash = hash;
            this.peer = peer;
            this.options = options;
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public PeerHash Peer
        {
            get { return peer; }
        }

        public HandshakeOptions Options
        {
            get { return options; }
        }

        public void OnHandshake(NetworkConnection negotiated, Handshake handshake)
        {
        }

        public void OnDisconnected()
        {
        }
    }
}