using Leak.Networking.Core;
using Leak.Tasks;

namespace Leak.Networking
{
    internal class NetworkPoolDecrypt : NetworkPoolTask
    {
        private readonly NetworkPoolListener listener;
        private readonly long identifier;
        private readonly NetworkIncomingBuffer buffer;
        private readonly NetworkIncomingMessageHandler handler;
        private readonly int count;

        public NetworkPoolDecrypt(NetworkPoolListener listener, long identifier, NetworkIncomingBuffer buffer, NetworkIncomingMessageHandler handler, int count)
        {
            this.listener = listener;
            this.identifier = identifier;
            this.buffer = buffer;
            this.handler = handler;
            this.count = count;
        }

        public bool CanExecute(NetworkPoolQueue queue)
        {
            return true;
        }

        public void Execute(NetworkPoolInstance context, NetworkPoolTaskCallback callback)
        {
            listener.HandleReceived(identifier, count);
            buffer.Process(handler, count);
        }

        public void Block(NetworkPoolQueue queue)
        {
        }

        public void Release(NetworkPoolQueue queue)
        {
        }
    }
}