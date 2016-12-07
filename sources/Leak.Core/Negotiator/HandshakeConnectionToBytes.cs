using Leak.Core.Network;
using System;

namespace Leak.Core.Negotiator
{
    public class HandshakeConnectionToBytes : NetworkIncomingMessageHandler
    {
        private readonly HandshakeNegotiatorContext context;
        private readonly Action<NetworkIncomingMessage> callback;
        private readonly int bytes;

        public HandshakeConnectionToBytes(HandshakeNegotiatorContext context, Action<NetworkIncomingMessage> callback, int bytes)
        {
            this.context = context;
            this.callback = callback;
            this.bytes = bytes;
        }

        public void OnMessage(NetworkIncomingMessage message)
        {
            if (message.Length < bytes)
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
            context.OnDisconnected();
        }
    }
}