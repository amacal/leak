using Leak.Core.Network;
using System;

namespace Leak.Core.Negotiator
{
    public class HandshakeConnectionToPeek : NetworkIncomingMessageHandler
    {
        private readonly HandshakeNegotiatorContext context;
        private readonly Action<NetworkIncomingMessage> callback;
        private readonly Func<NetworkIncomingMessage, bool> peek;

        public HandshakeConnectionToPeek(HandshakeNegotiatorContext context, Action<NetworkIncomingMessage> callback, Func<NetworkIncomingMessage, bool> peek)
        {
            this.context = context;
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

        public void OnException(Exception ex)
        {
            context.OnException(ex);
        }

        public void OnDisconnected()
        {
            context.OnDisconnected();
        }
    }
}