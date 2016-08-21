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
            configuration = configurer.Configure(with =>
            {
                with.Callback = new PeerConnectorCallbackNothing();
                with.Peer = new PeerHash(Bytes.Random(20));
                with.Pool = new NetworkPool();
            });
        }

        public void ConnectTo(PeerAddress peer)
        {
            if (OnConnecting(peer))
            {
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                EndPoint endpoint = new DnsEndPoint(peer.Host, peer.Port);

                socket.BeginConnect(endpoint, OnConnected, socket);
            }
        }

        private bool OnConnecting(PeerAddress peer)
        {
            bool accepted = true;

            NetworkConnectionInfo connection = configuration.Pool.Info(peer.ToString(), NetworkDirection.Incoming);
            PeerConnectorConnecting connecting = new PeerConnectorConnecting(configuration.Hash, connection, () => accepted = false);

            configuration.Callback.OnConnecting(connecting);
            return accepted;
        }

        private void OnConnected(IAsyncResult result)
        {
            try
            {
                Socket socket = (Socket)result.AsyncState;
                socket.EndConnect(result);

                NetworkConnection connection = configuration.Pool.Create(socket, NetworkDirection.Outgoing);
                PeerConnectorConnected connected = new PeerConnectorConnected(configuration.Hash, connection);

                configuration.Callback.OnConnected(connected);

                PeerConnectorNegotiatorContext context = new PeerConnectorNegotiatorContext(configuration, connection);
                HandshakeNegotiatorActive negotiator = new HandshakeNegotiatorActive(configuration.Pool, connection, context);

                negotiator.Execute();
            }
            catch (SocketException)
            {
            }
        }
    }
}