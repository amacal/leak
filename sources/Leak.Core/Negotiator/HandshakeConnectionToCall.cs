using Leak.Core.Network;
using System;
using Leak.Common;

namespace Leak.Core.Negotiator
{
    public class HandshakeConnectionToCall : NetworkIncomingMessageHandler
    {
        private readonly HandshakeNegotiatorContext context;
        private readonly Action<NetworkIncomingMessage> callback;

        public HandshakeConnectionToCall(HandshakeNegotiatorContext context, Action<NetworkIncomingMessage> callback)
        {
            this.context = context;
            this.callback = callback;
        }

        public void OnMessage(NetworkIncomingMessage message)
        {
            callback.Invoke(message);
        }

        public void OnDisconnected()
        {
            context.OnDisconnected();
        }
    }
}