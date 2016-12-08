using Leak.Sockets;
using System.Net;
using Leak.Common;
using Leak.Tasks;

namespace Leak.Core.Connector
{
    public class PeerConnectorTaskConnect : LeakTask<PeerConnectorContext>
    {
        private readonly FileHash hash;
        private readonly PeerAddress remote;

        public PeerConnectorTaskConnect(FileHash hash, PeerAddress remote)
        {
            this.hash = hash;
            this.remote = remote;
        }

        public void Execute(PeerConnectorContext context)
        {
            TcpSocket socket = context.Pool.New();
            IPAddress[] addresses = Dns.GetHostAddresses(remote.Host);

            IPAddress address = addresses[0].MapToIPv4();
            IPEndPoint endpoint = new IPEndPoint(address, remote.Port);

            socket.Bind();
            socket.Connect(endpoint, data => OnConnected(context, data));
        }

        private void OnConnected(PeerConnectorContext context, TcpSocketConnect data)
        {
            if (data.Status == TcpSocketStatus.OK)
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