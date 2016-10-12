using System.Net;

namespace Leak.Suckets
{
    public delegate void TcpSocketAcceptParse(out IPEndPoint local, out IPEndPoint remote);
}