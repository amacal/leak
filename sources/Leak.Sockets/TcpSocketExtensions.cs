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
    }
}