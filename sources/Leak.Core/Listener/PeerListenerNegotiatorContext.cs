using Leak.Core.Negotiator;
using Leak.Core.Network;
using System;
using Leak.Common;

namespace Leak.Core.Listener
{
    public class PeerListenerNegotiatorContext : HandshakeNegotiatorPassiveContext
    {
        private readonly PeerListenerConfiguration configuration;
        private readonly NetworkConnection connection;
        private readonly FileHashCollection hashes;
        private readonly PeerListenerHooks hooks;

        public PeerListenerNegotiatorContext(NetworkConnection connection, FileHashCollection hashes, PeerListenerHooks hooks, PeerListenerConfiguration configuration)
        {
            this.configuration = configuration;
            this.connection = connection;
            this.hashes = hashes;
            this.hooks = hooks;
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
            get { return hashes; }
        }

        public void OnRejected(HandshakeRejection rejection)
        {
            hooks.CallHandshakeRejected(connection);
        }

        public void OnHandshake(NetworkConnection negotiated, Handshake handshake)
        {
            hooks.CallHandshakeCompleted(negotiated, handshake);
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