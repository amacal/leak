using Leak.Sockets;

namespace Leak.Listener
{
    public abstract class PeerListenerPort
    {
        protected abstract TcpSocketInfo Execute(TcpSocket socket);

        public int? Bind(TcpSocket socket)
        {
            TcpSocketInfo info = Execute(socket);
            int? port = info?.Endpoint.Port;

            return port;
        }
    }
}