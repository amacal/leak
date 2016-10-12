using Leak.Core.Common;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using Leak.Suckets;
using System.Net;
using System.Net.Sockets;

namespace Leak.Core.Connector
{
    public class PeerConnectorTaskConnect : PeerConnectorTask
    {
        private readonly PeerConnectorContext context;
        private readonly FileHash hash;
        private readonly PeerAddress peer;

        public PeerConnectorTaskConnect(PeerConnectorContext context, FileHash hash, PeerAddress peer)
        {
            this.context = context;
            this.hash = hash;
            this.peer = peer;
        }

        public void Execute()
        {
            if (OnConnecting())
            {
                TcpSocket socket = context.Configuration.Pool.New();
                IPAddress[] addresses = Dns.GetHostAddresses(peer.Host);

                IPAddress address = addresses[0].MapToIPv4();
                IPEndPoint endpoint = new IPEndPoint(address, peer.Port);

                socket.Bind();
                socket.Connect(endpoint, OnConnected);
            }
        }

        private bool OnConnecting()
        {
            bool accepted = true;

            NetworkConnectionInfo connection = context.Pool.Info(peer.ToString(), NetworkDirection.Incoming);
            PeerConnectorConnecting connecting = new PeerConnectorConnecting(hash, connection, () => accepted = false);

            context.Configuration.Callback.OnConnecting(connecting);
            return accepted;
        }

        private void OnConnected(TcpSocketConnect data)
        {
            if (data.Status == TcpSocketStatus.OK)
            {
                PeerConnectorConnected connected;
                NetworkConnection connection = context.Pool.Create(data.Socket, NetworkDirection.Outgoing, data.Endpoint);

                try
                {
                    connected = new PeerConnectorConnected(hash, connection);
                    context.Configuration.Callback.OnConnected(connected);

                    PeerConnectorNegotiatorContext forNegotiator = new PeerConnectorNegotiatorContext(hash, context.Configuration, connection);
                    HandshakeNegotiatorActive negotiator = new HandshakeNegotiatorActive(context.Pool, connection, forNegotiator);

                    negotiator.Execute();
                }
                catch (SocketException ex)
                {
                    if (connection != null)
                    {
                        context.Configuration.Callback.OnException(connection, ex);
                    }
                }
            }
        }
    }
}