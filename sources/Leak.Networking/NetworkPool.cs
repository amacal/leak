using Leak.Sockets;
using System.Net;
using Leak.Networking.Core;

namespace Leak.Networking
{
    public interface NetworkPool
    {
        void Start();

        TcpSocket New();

        NetworkConnection Create(TcpSocket socket, NetworkDirection direction, IPEndPoint remote);

        NetworkConnection Change(NetworkConnection connection, NetworkConfiguration configurer);
    }
}