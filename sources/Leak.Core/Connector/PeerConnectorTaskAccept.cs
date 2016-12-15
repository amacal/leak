using Leak.Common;
using Leak.Negotiator;
using Leak.Sockets;
using Leak.Tasks;
using System.Net;

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
            NetworkConnection connection = context.Pool.Create(socket, NetworkDirection.Outgoing, endpoint);

            context.Hooks.CallConnectionEstablished(connection);

            PeerConnectorNegotiatorContext forNegotiator = new PeerConnectorNegotiatorContext(hash, context, connection);
            HandshakeNegotiatorActive negotiator = new HandshakeNegotiatorActive(context.Pool, connection, forNegotiator, new HandshakeNegotiatorHooks());

            negotiator.Execute();
        }
    }
}