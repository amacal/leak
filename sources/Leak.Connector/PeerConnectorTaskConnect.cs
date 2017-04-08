using Leak.Common;
using Leak.Sockets;
using Leak.Tasks;
using System.Net;
using Leak.Networking.Core;

namespace Leak.Connector
{
    public class PeerConnectorTaskConnect : LeakTask<PeerConnectorContext>
    {
        private readonly FileHash hash;
        private readonly NetworkAddress remote;

        public PeerConnectorTaskConnect(FileHash hash, NetworkAddress remote)
        {
            this.hash = hash;
            this.remote = remote;
        }

        public void Execute(PeerConnectorContext context)
        {
            TcpSocket socket = context.Dependencies.Network.New();
            IPAddress[] addresses = Dns.GetHostAddresses(remote.Host);

            IPAddress address = addresses[0].MapToIPv4();
            IPEndPoint endpoint = new IPEndPoint(address, remote.Port);

            socket.Bind();
            socket.Connect(endpoint, data => OnConnected(context, data));
        }

        private void OnConnected(PeerConnectorContext context, TcpSocketConnect data)
        {
            if (data.Status == SocketStatus.OK)
            {
                context.Queue.Add(new PeerConnectorTaskAccept(hash, data.Socket, data.Endpoint));
            }
            else
            {
                context.Queue.Add(new PeerConnectorTaskReject(remote));
                data.Socket.Dispose();
            }
        }
    }
}