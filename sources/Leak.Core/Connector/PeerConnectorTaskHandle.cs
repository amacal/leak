using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using Leak.Suckets;
using System.Net;

namespace Leak.Core.Connector
{
    public class PeerConnectorTaskHandle : LeakTask<PeerConnectorContext>
    {
        private readonly FileHash hash;
        private readonly TcpSocket socket;
        private readonly IPEndPoint endpoint;

        public PeerConnectorTaskHandle(FileHash hash, TcpSocket socket, IPEndPoint endpoint)
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
            context.Configuration.Callback.OnConnected(connected);

            PeerConnectorNegotiatorContext forNegotiator = new PeerConnectorNegotiatorContext(hash, context.Configuration, connection);
            HandshakeNegotiatorActive negotiator = new HandshakeNegotiatorActive(context.Pool, connection, forNegotiator);

            negotiator.Execute();
        }
    }
}