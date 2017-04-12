using System;

namespace Leak.Networking.Core
{
    public class NetworkPredicateHandler : NetworkIncomingMessageHandler
    {
        private readonly Action<NetworkIncomingMessage> callback;
        private readonly Func<NetworkIncomingMessage, bool> predicate;
        private readonly Action onDisconnected;

        public NetworkPredicateHandler(Action<NetworkIncomingMessage> callback, Func<NetworkIncomingMessage, bool> predicate, Action onDisconnected)
        {
            this.callback = callback;
            this.predicate = predicate;
            this.onDisconnected = onDisconnected;
        }

        public void OnMessage(NetworkIncomingMessage message)
        {
            if (predicate.Invoke(message) == false)
            {
                message.Continue(this);
            }
            else
            {
                callback.Invoke(message);
            }
        }

        public void OnDisconnected()
        {
            onDisconnected?.Invoke();
        }
    }
}