using System;
using System.Net;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public class PeerListener
    {
        private readonly Socket socket;
        private readonly PeerListenerConfiguration configuration;

        public PeerListener(Action<PeerListenerConfiguration> callback)
        {
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            configuration = new PeerListenerConfiguration
            {
                Port = 8080
            };

            callback.Invoke(configuration);
        }

        public void Listen()
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, configuration.Port));
            socket.Listen(1000);
            socket.BeginAccept(OnAccepted, null);
        }

        public void Stop()
        {
            socket.Disconnect(true);
        }

        private void OnAccepted(IAsyncResult result)
        {
            try
            {
                Socket endpoint = socket.EndAccept(result);
                PeerNegotiator negotiator = configuration.Negotiator;

                PeerConnection connection = new PeerConnection(endpoint);
                PeerNegotiatable negotiable = new PeerNegotiatable(connection, configuration);

                negotiator.Passive(negotiable);
            }
            catch (SocketException)
            {
            }
        }

        private class PeerNegotiatable : PeerNegotiatorPassiveContext
        {
            private readonly PeerConnection connection;
            private readonly PeerListenerConfiguration configuration;

            public PeerNegotiatable(PeerConnection connection, PeerListenerConfiguration configuration)
            {
                this.connection = connection;
                this.configuration = configuration;
            }

            public PeerConnection Connection
            {
                get { return connection; }
            }

            public PeerNegotiatorHashCollection Hashes
            {
                get { return configuration.Hashes; }
            }

            public void Continue(PeerHandshakePayload handshake, PeerConnection connection)
            {
                configuration.Callback.OnHandshake(connection, new PeerHandshake(connection, handshake));
            }

            public void Terminate()
            {
            }
        }

        private class ClientDescription : PeerDescription
        {
            private readonly EndPoint endpoint;

            public ClientDescription(EndPoint endpoint)
            {
                this.endpoint = endpoint;
            }

            public override string Host
            {
                get { return endpoint.ToString(); }
            }

            public override string ToString()
            {
                return $"< {endpoint}";
            }
        }
    }
}