using Leak.Common;
using Leak.Tasks;

namespace Leak.Networking
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