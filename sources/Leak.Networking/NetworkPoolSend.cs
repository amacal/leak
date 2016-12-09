using Leak.Sockets;
using Leak.Tasks;

namespace Leak.Networking
{
    public class NetworkPoolSend : LeakTask<NetworkPoolInstance>
    {
        private readonly TcpSocket socket;
        private readonly byte[] data;

        public NetworkPoolSend(TcpSocket socket, byte[] data)
        {
            this.socket = socket;
            this.data = data;
        }

        public void Execute(NetworkPoolInstance context)
        {
            socket.Send(data, null);
        }
    }
}