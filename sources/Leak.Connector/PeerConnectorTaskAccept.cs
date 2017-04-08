using Leak.Common;
using Leak.Networking;
using Leak.Sockets;
using Leak.Tasks;
using System.Net;
using Leak.Networking.Core;

namespace Leak.Connector
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
            NetworkPool pool = context.Dependencies.Network;
            NetworkConnection connection = pool.Create(socket, NetworkDirection.Outgoing, endpoint);

            context.Hooks.CallConnectionEstablished(connection);
        }
    }
}