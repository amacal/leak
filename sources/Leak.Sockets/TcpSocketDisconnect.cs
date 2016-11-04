namespace Leak.Sockets
{
    public class TcpSocketDisconnect
    {
        private readonly TcpSocketStatus status;
        private readonly TcpSocket socket;

        public TcpSocketDisconnect(TcpSocketStatus status, TcpSocket socket)
        {
            this.status = status;
            this.socket = socket;
        }

        public TcpSocketStatus Status
        {
            get { return status; }
        }

        public TcpSocket Socket
        {
            get { return socket; }
        }
    }
}