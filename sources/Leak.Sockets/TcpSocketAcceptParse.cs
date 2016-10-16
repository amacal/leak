using System.Net;

namespace Leak.Sockets
{
    public delegate void TcpSocketAcceptParse(out IPEndPoint local, out IPEndPoint remote);
}