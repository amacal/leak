using Leak.Common;
using Leak.Tasks;

namespace Leak.Networking
{
    public class NetworkPoolDecrypt : LeakTask<NetworkPoolInstance>
    {
        private readonly NetworkBuffer buffer;
        private readonly NetworkIncomingMessageHandler handler;
        private readonly int count;

        public NetworkPoolDecrypt(NetworkBuffer buffer, NetworkIncomingMessageHandler handler, int count)
        {
            this.buffer = buffer;
            this.handler = handler;
            this.count = count;
        }

        public void Execute(NetworkPoolInstance context)
        {
            buffer.Process(handler, count);
        }
    }
}