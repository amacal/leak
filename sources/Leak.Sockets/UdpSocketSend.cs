namespace Leak.Sockets
{
    public class UdpSocketSend
    {
        private readonly SocketStatus status;
        private readonly int count;
        private readonly UdpSocket socket;
        private readonly SocketBuffer buffer;

        public UdpSocketSend(SocketStatus status, int count, UdpSocket socket, SocketBuffer buffer)
        {
            this.status = status;
            this.count = count;
            this.socket = socket;
            this.buffer = buffer;
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

        public int Count
        {
            get { return count; }
        }
    }
}