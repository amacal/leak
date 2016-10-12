using System.Net;

namespace Leak.Suckets
{
    public class TcpSocketAccept
    {
        private readonly TcpSocketStatus status;
        private readonly TcpSocket socket;
        private readonly TcpSocket connection;
        private readonly TcpSocketAcceptParse parse;

        public TcpSocketAccept(TcpSocketStatus status, TcpSocket socket, TcpSocket connection, TcpSocketAcceptParse parse)
        {
            this.status = status;
            this.socket = socket;
            this.connection = connection;
            this.parse = parse;
        }

        public TcpSocketStatus Status
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
            IPEndPoint local;
            IPEndPoint remote;

            parse.Invoke(out local, out remote);
            return remote;
        }
    }
}