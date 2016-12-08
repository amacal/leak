using Leak.Sockets;
using Leak.Tasks;

namespace Leak.Networking
{
    public class NetworkPoolSend : LeakTask<NetworkPool>
    {
        private readonly long identifier;
        private readonly TcpSocket socket;
        private readonly byte[] data;

        public NetworkPoolSend(long identifier, TcpSocket socket, byte[] data)
        {
            this.identifier = identifier;
            this.socket = socket;
            this.data = data;
        }

        public void Execute(NetworkPool context)
        {
            socket.Send(data, null);
        }
    }
}