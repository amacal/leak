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
            socket.Bind(port);
            return socket.Info();
        }

        public static void Bind(this TcpSocket socket, out int port)
        {
            TcpSocketInfo info;
            socket.Bind();

            info = socket.Info();
            port = info.Endpoint.Port;
        }

        public static Task<TcpSocketConnect> Connect(this TcpSocket socket, int port)
        {
            return socket.Connect(new IPEndPoint(IPAddress.Loopback, port));
        }
    }
}