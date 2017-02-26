using Leak.Sockets;
using Leak.Tasks;

namespace Leak.Networking
{
    public class NetworkPoolSend : LeakTask<NetworkPoolInstance>
    {
        private readonly NetworkPoolListener listener;
        private readonly long identifier;
        private readonly TcpSocket socket;
        private readonly byte[] data;

        public NetworkPoolSend(NetworkPoolListener listener, long identifier, TcpSocket socket, byte[] data)
        {
            this.listener = listener;
            this.identifier = identifier;
            this.socket = socket;
            this.data = data;
        }

        public void Execute(NetworkPoolInstance context)
        {
            if (listener.IsAvailable(identifier))
            {
                socket.Send(data, OnSent);
            }
        }

        private void OnSent(TcpSocketSend sent)
        {
            if (listener.IsAvailable(identifier))
            {
                if (sent.Status != SocketStatus.OK || sent.Count == 0)
                {
                    listener.Disconnect(identifier);
                }

                if (sent.Count > 0)
                {
                    listener.HandleSent(identifier, sent.Count);
                }
            }
        }
    }
}