namespace Leak.Sockets
{
    public class TcpSocketReceive
    {
        private readonly SocketStatus status;
        private readonly int count;
        private readonly TcpSocket socket;
        private readonly SocketBuffer buffer;

        public TcpSocketReceive(SocketStatus status, int count, TcpSocket socket, SocketBuffer buffer)
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

        public TcpSocket Socket
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