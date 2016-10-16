using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Network;
using Leak.Suckets;
using System.Net;
using Leak.Sockets;

namespace Leak.Core.Connector
{
    public class PeerConnectorTaskConnect : LeakTask<PeerConnectorContext>
    {
        private readonly FileHash hash;
        private readonly PeerAddress peer;

        public PeerConnectorTaskConnect(FileHash hash, PeerAddress peer)
        {
            this.hash = hash;
            this.peer = peer;
        }

        public void Execute(PeerConnectorContext context)
        {
            if (OnConnecting(context))
            {
                TcpSocket socket = context.Configuration.Pool.New();
                IPAddress[] addresses = Dns.GetHostAddresses(peer.Host);

                IPAddress address = addresses[0].MapToIPv4();
                IPEndPoint endpoint = new IPEndPoint(address, peer.Port);

                socket.Bind();
                socket.Connect(endpoint, data => OnConnected(context, data));
            }
        }

        private bool OnConnecting(PeerConnectorContext context)
        {
            bool accepted = true;

            NetworkConnectionInfo connection = new NetworkConnectionInfo(peer, NetworkDirection.Incoming);
            PeerConnectorConnecting connecting = new PeerConnectorConnecting(hash, connection, () => accepted = false);

            context.Configuration.Callback.OnConnecting(connecting);
            return accepted;
        }

        private void OnConnected(PeerConnectorContext context, TcpSocketConnect data)
        {
            if (data.Status == TcpSocketStatus.OK)
            {
                context.Queue.Add(new PeerConnectorTaskHandle(hash, data.Socket, data.Endpoint));
            }
            else
            {
                data.Socket.Dispose();
            }
        }
    }
}