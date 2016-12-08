using Leak.Common;
using Leak.Core.Core;

namespace Leak.Core.Network
{
    public class NetworkPoolDecrypt : LeakTask<NetworkPool>
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

        public void Execute(NetworkPool context)
        {
            buffer.Process(handler, count);
        }
    }
}