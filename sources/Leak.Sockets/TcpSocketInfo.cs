using System.Net;

namespace Leak.Sockets
{
    public class TcpSocketInfo
    {
        private readonly SocketStatus status;
        private readonly TcpSocket socket;
        private readonly IPEndPoint endpoint;

        public TcpSocketInfo(SocketStatus status, TcpSocket socket, IPEndPoint endpoint)
        {
            this.status = status;
            this.socket = socket;
            this.endpoint = endpoint;
        }

        public SocketStatus Status
        {
            get { return status; }
        }

        public TcpSocket Socket
        {
            get { return socket; }
        }

        public IPEndPoint Endpoint
        {
            get { return endpoint; }
        }
    }
}