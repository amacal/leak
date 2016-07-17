using Leak.Core.Common;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using System;
using System.Net;
using System.Net.Sockets;

namespace Leak.Core.Connector
{
    public class PeerConnector
    {
        private readonly PeerConnectorConfiguration configuration;

        public PeerConnector(Action<PeerConnectorConfiguration> configurer)
        {
            this.configuration = new PeerConnectorConfiguration
            {
                Callback = new PeerConnectorCallbackToNothing(),
                Peer = new PeerHash(Bytes.Random(20))
            };

            configurer.Invoke(configuration);
        }

        public void ConnectTo(string host, int port)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            EndPoint endpoint = new DnsEndPoint(host, port);

            socket.BeginConnect(endpoint, OnConnected, socket);
        }

        private void OnConnected(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            socket.EndConnect(result);

            NetworkConnection connection = new NetworkConnection(socket, NetworkConnectionDirection.Outgoing);
            configuration.Callback.OnConnected(connection);

            PeerConnectorNegotiatorContext context = new PeerConnectorNegotiatorContext(configuration, connection);
            HandshakeNegotiatorActive negotiator = new HandshakeNegotiatorActive(connection, context);

            negotiator.Execute();
        }
    }
}