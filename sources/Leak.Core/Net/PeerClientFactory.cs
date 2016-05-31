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

            public PeerHandshakeOptions Options
            {
                get { return PeerHandshakeOptions.None; }
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