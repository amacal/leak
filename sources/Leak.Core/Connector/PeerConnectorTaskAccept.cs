using Leak.Core.Core;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using Leak.Sockets;
using System.Net;
using Leak.Common;

namespace Leak.Core.Connector
{
    public class PeerConnectorTaskAccept : LeakTask<PeerConnectorContext>
    {
        private readonly FileHash hash;
        private readonly TcpSocket socket;
        private readonly IPEndPoint endpoint;

        public PeerConnectorTaskAccept(FileHash hash, TcpSocket socket, IPEndPoint endpoint)
        {
            this.hash = hash;
            this.socket = socket;
            this.endpoint = endpoint;
        }

        public void Execute(PeerConnectorContext context)
        {
            PeerConnectorConnected connected;
            NetworkConnection connection = context.Pool.Create(socket, NetworkDirection.Outgoing, endpoint);

            connected = new PeerConnectorConnected(hash, connection);
            context.Hooks.CallConnectionEstablished(connection);

            PeerConnectorNegotiatorContext forNegotiator = new PeerConnectorNegotiatorContext(hash, context, connection);
            HandshakeNegotiatorActive negotiator = new HandshakeNegotiatorActive(context.Pool, connection, forNegotiator);

            negotiator.Execute();
        }
    }
}