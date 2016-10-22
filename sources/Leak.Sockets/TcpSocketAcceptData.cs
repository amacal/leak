namespace Leak.Suckets
{
    public class TcpSocketAcceptData
    {
        private readonly TcpSocket socket;
        private readonly TcpSocket connection;

        public TcpSocketAcceptData(TcpSocket socket, TcpSocket connection)
        {
            this.socket = socket;
            this.connection = connection;
        }

        public TcpSocket Socket
        {
            get { return socket; }
        }

        public TcpSocket Connection
        {
            get { return connection; }
        }
    }
}