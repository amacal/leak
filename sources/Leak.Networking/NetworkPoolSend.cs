using System;
using Leak.Networking.Core;

namespace Leak.Networking
{
    internal class NetworkPoolSend : NetworkPoolTask
    {
        private readonly long identifier;
        private readonly NetworkOutgoingBuffer buffer;
        private readonly NetworkOutgoingMessage message;

        public NetworkPoolSend(long identifier, NetworkOutgoingBuffer buffer, NetworkOutgoingMessage message)
        {
            this.identifier = identifier;
            this.buffer = buffer;
            this.message = message;
        }

        public bool CanExecute(NetworkPoolQueue queue)
        {
            return queue.IsBlocked(identifier) == false &&
                   buffer.IsAvailable(message.Length);
        }

        public void Execute(NetworkPoolInstance context, NetworkPoolTaskCallback callback)
        {
            buffer.Send(message, OnExecuted(callback));
        }

        private Action OnExecuted(NetworkPoolTaskCallback onCompleted)
        {
            return () =>
            {
                message.Release();
                onCompleted.Invoke(this);
            };
        }

        public void Block(NetworkPoolQueue queue)
        {
            queue.Block(identifier);
        }

        public void Release(NetworkPoolQueue queue)
        {
            queue.Release(identifier);
        }
    }
}