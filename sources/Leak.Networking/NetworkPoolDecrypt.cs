using Leak.Common;
using Leak.Tasks;

namespace Leak.Networking
{
    public class NetworkPoolDecrypt : LeakTask<NetworkPoolInstance>
    {
        private readonly NetworkPoolListener listener;
        private readonly long identifier;
        private readonly NetworkBuffer buffer;
        private readonly NetworkIncomingMessageHandler handler;
        private readonly int count;

        public NetworkPoolDecrypt(NetworkPoolListener listener, long identifier, NetworkBuffer buffer, NetworkIncomingMessageHandler handler, int count)
        {
            this.listener = listener;
            this.identifier = identifier;
            this.buffer = buffer;
            this.handler = handler;
            this.count = count;
        }

        public void Execute(NetworkPoolInstance context)
        {
            listener.HandleReceived(identifier, count);
            buffer.Process(handler, count);
        }
    }
}