using System;
using System.Net;
using System.Net.Sockets;
using Leak.Core.Network;

namespace Leak.Core.Net
{
    public class PeerListener : IDisposable
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
            if (socket.Connected)
            {
                socket.Disconnect(true);
            }
        }

        public void Dispose()
        {
            socket.Close();
            socket.Dispose();
        }

        private void OnAccepted(IAsyncResult result)
        {
            try
            {
                Socket endpoint = socket.EndAccept(result);
                PeerNegotiator negotiator = configuration.Negotiator;

                NetworkConnection connection = new NetworkConnection(endpoint, NetworkConnectionDirection.Incoming);
                PeerNegotiatable negotiable = new PeerNegotiatable(connection, configuration);

                configuration.Callback.OnConnect(connection);
                negotiator.Passive(negotiable);
            }
            catch (SocketException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private class PeerNegotiatable : PeerNegotiatorPassiveContext
        {
            private readonly NetworkConnection connection;
            private readonly PeerListenerConfiguration configuration;

            public PeerNegotiatable(NetworkConnection connection, PeerListenerConfiguration configuration)
            {
                this.connection = connection;
                this.configuration = configuration;
            }

            public NetworkConnection Connection
            {
                get { return connection; }
            }

            public PeerHandshakeOptions Options
            {
                get { return configuration.Options; }
            }

            public PeerNegotiatorHashCollection Hashes
            {
                get { return configuration.Hashes; }
            }

            public void Continue(PeerHandshakePayload handshake, NetworkConnection connection)
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