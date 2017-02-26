namespace Leak.Sockets
{
    public class TcpSocketDisconnect
    {
        private readonly SocketStatus status;
        private readonly TcpSocket socket;

        public TcpSocketDisconnect(SocketStatus status, TcpSocket socket)
        {
            this.status = status;
            this.socket = socket;
        }

        public SocketStatus Status
        {
            get { return status; }
        }

        public TcpSocket Socket
        {
            get { return socket; }
        }
    }
}