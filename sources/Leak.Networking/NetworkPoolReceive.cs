using Leak.Networking.Core;

namespace Leak.Networking
{
    internal class NetworkPoolReceive : NetworkPoolTask
    {
        private readonly NetworkIncomingMessageHandler handler;
        private readonly NetworkIncomingBufferMessage message;

        public NetworkPoolReceive(NetworkIncomingMessageHandler handler, NetworkIncomingBufferMessage message)
        {
            this.handler = handler;
            this.message = message;
        }

        public bool CanExecute(NetworkPoolQueue queue)
        {
            return true;
        }

        public void Execute(NetworkPoolInstance context, NetworkPoolTaskCallback callback)
        {
            handler.OnMessage(message);
        }

        public void Block(NetworkPoolQueue queue)
        {
        }

        public void Release(NetworkPoolQueue queue)
        {
        }
    }
}