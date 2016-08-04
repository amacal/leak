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
                Callback = new ConnectionLoopCallbackNothing()
            };

            configurer.Invoke(configuration);
        }

        public void StartProcessing(NetworkConnection connection, PeerListenerHandshake handshake)
        {
            StartProcessing(connection, new ConnectionLoopHandshakeToListener(handshake));
        }

        public void StartProcessing(NetworkConnection connection, PeerConnectorHandshake handshake)
        {
            StartProcessing(connection, new ConnectionLoopHandshakeToConnector(handshake));
        }

        private void StartProcessing(NetworkConnection network, ConnectionLoopHandshake handshake)
        {
            ConnectionLoopConnection connection = new ConnectionLoopConnection(configuration, network, handshake);
            ConnectionLoopHandler handler = new ConnectionLoopHandler(configuration, connection, handshake);

            handler.Execute();
        }
    }
}