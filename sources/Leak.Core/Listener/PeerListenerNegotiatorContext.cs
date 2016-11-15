using Leak.Core.Common;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using System;

namespace Leak.Core.Listener
{
    public class PeerListenerNegotiatorContext : HandshakeNegotiatorPassiveContext
    {
        private readonly PeerListenerConfiguration configuration;
        private readonly NetworkConnection connection;

        public PeerListenerNegotiatorContext(PeerListenerConfiguration configuration, NetworkConnection connection)
        {
            this.configuration = configuration;
            this.connection = connection;
        }

        public PeerHash Peer
        {
            get { return configuration.Peer; }
        }

        public HandshakeOptions Options
        {
            get
            {
                HandshakeOptions options = HandshakeOptions.None;

                if (configuration.Extensions)
                {
                    options = options | HandshakeOptions.Extended;
                }

                return options;
            }
        }

        public FileHashCollection Hashes
        {
            get { return null; }
        }

        public void OnRejected(HandshakeRejection rejection)
        {
            //configuration.Callback.OnRejected(connection);
        }

        public void OnHandshake(NetworkConnection negotiated, Handshake handshake)
        {
            //configuration.Callback.OnHandshake(negotiated, new PeerListenerHandshake(handshake));
        }

        public void OnException(Exception ex)
        {
            //configuration.Callback.OnException(connection, ex);
        }

        public void OnDisconnected()
        {
            //configuration.Callback.OnDisconnected(connection);
        }
    }
}