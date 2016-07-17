using Leak.Core.Connector;
using Leak.Core.Listener;
using Leak.Core.Network;
using System;

namespace Leak.Core.Loop
{
    public class ConnectionLoop
    {
        private readonly ConnectionLoopConfiguration configuration;

        public ConnectionLoop(Action<ConnectionLoopConfiguration> configurer)
        {
            this.configuration = new ConnectionLoopConfiguration
            {
                Callback = new ConnectionLoopCallbackToNothing()
            };

            configurer.Invoke(configuration);
        }

        public void Handle(NetworkConnection connection, PeerListenerHandshake handshake)
        {
            Handle(connection, new ConnectionLoopHandshakeToListener(handshake));
        }

        public void Handle(NetworkConnection connection, PeerConnectorHandshake handshake)
        {
            Handle(connection, new ConnectionLoopHandshakeToConnector(handshake));
        }

        private void Handle(NetworkConnection network, ConnectionLoopHandshake handshake)
        {
            ConnectionLoopConnection connection = new ConnectionLoopConnection(configuration, network, handshake);
            ConnectionLoopHandler handler = new ConnectionLoopHandler(configuration, connection, handshake);

            handler.Execute();
        }
    }
}