namespace Leak.Sockets
{
    public class TcpSocketReceive
    {
        private readonly TcpSocketStatus status;
        private readonly int count;
        private readonly TcpSocket socket;
        private readonly TcpSocketBuffer buffer;

        public TcpSocketReceive(TcpSocketStatus status, int count, TcpSocket socket, TcpSocketBuffer buffer)
        {
            this.status = status;
            this.count = count;
            this.socket = socket;
            this.buffer = buffer;
        }

        public TcpSocketStatus Status
        {
            get { return status; }
        }

        public TcpSocket Socket
        {
            get { return socket; }
        }

        public TcpSocketBuffer Buffer
        {
            get { return buffer; }
        }

        public int Count
        {
            get { return count; }
        }
    }
}