using Leak.Core.Core;
using Leak.Sockets;

namespace Leak.Core.Network
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