using Leak.Common;
using Leak.Core.Core;

namespace Leak.Core.Network
{
    public class NetworkPoolReceive : LeakTask<NetworkPool>
    {
        private readonly NetworkIncomingMessageHandler handler;
        private readonly NetworkBufferMessage message;

        public NetworkPoolReceive(NetworkIncomingMessageHandler handler, NetworkBufferMessage message)
        {
            this.handler = handler;
            this.message = message;
        }

        public void Execute(NetworkPool context)
        {
            handler.OnMessage(message);
        }
    }
}