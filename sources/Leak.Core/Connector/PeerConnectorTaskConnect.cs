using Leak.Core.Common;
using Leak.Core.Network;
using Leak.Suckets;
using System.Net;

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

            NetworkConnectionInfo connection = new NetworkConnectionInfo(peer.ToString(), NetworkDirection.Incoming);
            PeerConnectorConnecting connecting = new PeerConnectorConnecting(hash, connection, () => accepted = false);

            context.Configuration.Callback.OnConnecting(connecting);
            return accepted;
        }

        private void OnConnected(TcpSocketConnect data)
        {
            if (data.Status == TcpSocketStatus.OK)
            {
                context.Queue.Add(new PeerConnectorTaskHandle(context, hash, data.Socket, data.Endpoint));
            }
        }
    }
}