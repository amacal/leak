using System.Net;
using System.Threading.Tasks;

namespace Leak.Sockets
{
    public static class TcpSocketExtensions
    {
        public static TcpSocketInfo BindAndInfo(this TcpSocket socket)
        {
            socket.Bind();
            return socket.Info();
        }

        public static TcpSocketInfo BindAndInfo(this TcpSocket socket, int port)
        {
            if (socket.Bind(port) == false)
                return null;

            return socket.Info();
        }

        public static bool Bind(this TcpSocket socket, out int? port)
        {
            TcpSocketInfo info;

            if (socket.Bind() == false)
            {
                port = null;
                return false;
            }

            info = socket.Info();
            port = info.Endpoint.Port;

            return true;
        }

        public static Task<TcpSocketConnect> Connect(this TcpSocket socket, int port)
        {
            return socket.Connect(new IPEndPoint(IPAddress.Loopback, port));
        }
    }
}