using Leak.Common;
using System;

namespace Leak.Negotiator
{
    public class HandshakeConnectionToPeek : NetworkIncomingMessageHandler
    {
        private readonly HandshakeNegotiatorHooks hooks;
        private readonly Action<NetworkIncomingMessage> callback;
        private readonly Func<NetworkIncomingMessage, bool> peek;

        public HandshakeConnectionToPeek(HandshakeNegotiatorHooks hooks, Action<NetworkIncomingMessage> callback, Func<NetworkIncomingMessage, bool> peek)
        {
            this.hooks = hooks;
            this.callback = callback;
            this.peek = peek;
        }

        public void OnMessage(NetworkIncomingMessage message)
        {
            if (peek.Invoke(message) == false)
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
            //context.OnDisconnected();
        }
    }
}