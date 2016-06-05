using System;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public class PeerClientFactory
    {
        private readonly PeerClientConfiguration configuration;

        public PeerClientFactory(Action<PeerClientConfiguration> configurer)
        {
            configuration = new PeerClientConfiguration();
            configurer.Invoke(configuration);
        }

        public void Connect(string host, int port)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            socket.BeginConnect(host, port, OnConnected, socket);
        }

        private void OnConnected(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            PeerConnection connection = new PeerConnection(socket);

            try
            {
                PeerNegotiator negotiator = configuration.Negotiator;
                PeerNegotiatable negotiable = new PeerNegotiatable(connection, configuration);

                socket.EndConnect(result);
                configuration.Callback.OnConnect(connection);
                negotiator.Active(negotiable);
            }
            catch (SocketException)
            {
                configuration.Callback.OnTerminate(connection);
            }
        }

        private class PeerNegotiatable : PeerNegotiatorActiveContext
        {
            private readonly PeerConnection connection;
            private readonly PeerClientConfiguration configuration;

            public PeerNegotiatable(PeerConnection connection, PeerClientConfiguration configuration)
            {
                this.connection = connection;
                this.configuration = configuration;
            }

            public byte[] Hash
            {
                get { return configuration.Hash; }
            }

            public PeerConnection Connection
            {
                get { return connection; }
            }

            public PeerHandshakeOptions Options
            {
                get { return configuration.Options; }
            }

            public void Continue(PeerHandshakePayload handshake, PeerConnection connection)
            {
                configuration.Callback.OnHandshake(connection, new PeerHandshake(connection, handshake));
            }

            public void Terminate()
            {
            }
        }
    }
}