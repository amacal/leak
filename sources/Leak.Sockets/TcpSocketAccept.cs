using System.Net;

namespace Leak.Sockets
{
    public class TcpSocketAccept
    {
        private readonly SocketStatus status;
        private readonly TcpSocket socket;
        private readonly TcpSocket connection;
        private readonly TcpSocketAcceptParse parse;

        public TcpSocketAccept(SocketStatus status, TcpSocket socket, TcpSocket connection, TcpSocketAcceptParse parse)
        {
            this.status = status;
            this.socket = socket;
            this.connection = connection;
            this.parse = parse;
        }

        public SocketStatus Status
        {
            get { return status; }
        }

        public TcpSocket Socket
        {
            get { return socket; }
        }

        public TcpSocket Connection
        {
            get { return connection; }
        }

        public IPEndPoint GetLocal()
        {
            IPEndPoint local;
            IPEndPoint remote;

            parse.Invoke(out local, out remote);
            return local;
        }

        public IPEndPoint GetRemote()
        {
            IPEndPoint local = null;
            IPEndPoint remote = null;

            parse?.Invoke(out local, out remote);
            return remote;
        }
    }
}