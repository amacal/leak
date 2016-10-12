using Leak.Core.Common;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using Leak.Suckets;
using System.Net;

namespace Leak.Core.Connector
{
    public class PeerConnectorTaskHandle : PeerConnectorTask
    {
        private readonly PeerConnectorContext context;
        private readonly FileHash hash;
        private readonly TcpSocket socket;
        private readonly IPEndPoint endpoint;

        public PeerConnectorTaskHandle(PeerConnectorContext context, FileHash hash, TcpSocket socket, IPEndPoint endpoint)
        {
            this.context = context;
            this.hash = hash;
            this.socket = socket;
            this.endpoint = endpoint;
        }

        public void Execute()
        {
            PeerConnectorConnected connected;
            NetworkConnection connection = context.Pool.Create(socket, NetworkDirection.Outgoing, endpoint);

            connected = new PeerConnectorConnected(hash, connection);
            context.Configuration.Callback.OnConnected(connected);

            PeerConnectorNegotiatorContext forNegotiator = new PeerConnectorNegotiatorContext(hash, context.Configuration, connection);
            HandshakeNegotiatorActive negotiator = new HandshakeNegotiatorActive(context.Pool, connection, forNegotiator);

            negotiator.Execute();
        }
    }
}