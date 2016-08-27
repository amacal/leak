using Leak.Core.Common;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using System;
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
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                EndPoint endpoint = new DnsEndPoint(peer.Host, peer.Port);

                socket.BeginConnect(endpoint, OnConnected, socket);
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

        private void OnConnected(IAsyncResult result)
        {
            try
            {
                Socket socket = (Socket)result.AsyncState;
                socket.EndConnect(result);

                NetworkConnection connection = context.Pool.Create(socket, NetworkDirection.Outgoing);
                PeerConnectorConnected connected = new PeerConnectorConnected(hash, connection);

                context.Configuration.Callback.OnConnected(connected);

                PeerConnectorNegotiatorContext forNegotiator = new PeerConnectorNegotiatorContext(hash, context.Configuration, connection);
                HandshakeNegotiatorActive negotiator = new HandshakeNegotiatorActive(context.Pool, connection, forNegotiator);

                negotiator.Execute();
            }
            catch (SocketException)
            {
            }
        }
    }
}