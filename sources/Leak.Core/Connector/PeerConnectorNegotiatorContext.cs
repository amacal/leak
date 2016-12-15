using Leak.Common;
using Leak.Negotiator;

namespace Leak.Core.Connector
{
    public class PeerConnectorNegotiatorContext : HandshakeNegotiatorActiveContext
    {
        private readonly FileHash hash;
        private readonly PeerConnectorContext context;
        private readonly NetworkConnection connection;

        public PeerConnectorNegotiatorContext(FileHash hash, PeerConnectorContext context, NetworkConnection connection)
        {
            this.hash = hash;
            this.context = context;
            this.connection = connection;
        }

        public PeerHash Peer
        {
            get { return context.Configuration.Peer; }
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public HandshakeOptions Options
        {
            get
            {
                HandshakeOptions options = HandshakeOptions.None;

                if (context.Configuration.Extensions)
                {
                    options = options | HandshakeOptions.Extended;
                }

                return options;
            }
        }

        public void OnHandshake(NetworkConnection negotiated, Handshake handshake)
        {
            context.Hooks.CallHandshakeCompleted(negotiated, handshake);
        }

        public void OnDisconnected()
        {
            context.Hooks.CallHandshakeRejected(connection);
        }
    }
}