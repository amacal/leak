using System.Net;

namespace Leak.Sockets
{
    public class UdpSocketSend
    {
        private readonly SocketStatus status;
        private readonly int count;
        private readonly UdpSocket socket;
        private readonly SocketBuffer buffer;
        private readonly IPEndPoint endpoint;

        public UdpSocketSend(SocketStatus status, int count, UdpSocket socket, SocketBuffer buffer, IPEndPoint endpoint)
        {
            this.status = status;
            this.count = count;
            this.socket = socket;
            this.buffer = buffer;
            this.endpoint = endpoint;
        }

        public SocketStatus Status
        {
            get { return status; }
        }

        public UdpSocket Socket
        {
            get { return socket; }
        }

        public SocketBuffer Buffer
        {
            get { return buffer; }
        }

        public IPEndPoint Endpoint
        {
            get { return endpoint; }
        }

        public int Count
        {
            get { return count; }
        }
    }
}