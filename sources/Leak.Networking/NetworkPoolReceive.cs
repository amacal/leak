using Leak.Common;
using Leak.Tasks;

namespace Leak.Networking
{
    public class NetworkPoolReceive : LeakTask<NetworkPoolInstance>
    {
        private readonly NetworkIncomingMessageHandler handler;
        private readonly NetworkIncomingBufferMessage message;

        public NetworkPoolReceive(NetworkIncomingMessageHandler handler, NetworkIncomingBufferMessage message)
        {
            this.handler = handler;
            this.message = message;
        }

        public void Execute(NetworkPoolInstance context)
        {
            handler.OnMessage(message);
        }
    }
}