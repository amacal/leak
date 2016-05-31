using System;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public class PeerClientFactory
    {
        private readonly Socket socket;
        private readonly PeerConnection connection;
        private readonly PeerClientConfiguration configuration;

        public PeerClientFactory(Action<PeerClientConfiguration> configurer)
        {
            this.socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            this.configuration = new PeerClientConfiguration();

            configurer.Invoke(this.configuration);
            this.connection = new PeerConnection(socket);
        }

        public void Connect(string host, int port)
        {
            socket.BeginConnect(host, port, OnConnected, null);
        }

        private void OnConnected(IAsyncResult result)
        {
            try
            {
                PeerNegotiator negotiator = configuration.Negotiator;
                PeerNegotiatable negotiable = new PeerNegotiatable(configuration.Hash, connection, configuration.Callback);

                socket.EndConnect(result);
                negotiator.Active(negotiable);
            }
            catch (SocketException)
            {
                configuration.Callback.OnTerminate(connection);
            }
        }

        private class PeerNegotiatable : PeerNegotiatorActiveContext
        {
            private readonly byte[] hash;
            private readonly PeerConnection connection;
            private readonly PeerNegotiatorCallback callback;

            public PeerNegotiatable(byte[] hash, PeerConnection connection, PeerNegotiatorCallback callback)
            {
                this.hash = hash;
                this.connection = connection;
                this.callback = callback;
            }

            public byte[] Hash
            {
                get { return hash; }
            }

            public PeerConnection Connection
            {
                get { return connection; }
            }

            public void Continue(PeerHandshakePayload handshake, PeerConnection connection)
            {
                callback.OnHandshake(connection, new PeerHandshake(connection, handshake));
            }

            public void Terminate()
            {
            }
        }
    }
}