using Leak.Common;
using System;

namespace Leak.Negotiator
{
    public class HandshakeConnectionToCall : NetworkIncomingMessageHandler
    {
        private readonly HandshakeNegotiatorHooks hooks;
        private readonly Action<NetworkIncomingMessage> callback;

        public HandshakeConnectionToCall(HandshakeNegotiatorHooks hooks, Action<NetworkIncomingMessage> callback)
        {
            this.hooks = hooks;
            this.callback = callback;
        }

        public void OnMessage(NetworkIncomingMessage message)
        {
            callback.Invoke(message);
        }

        public void OnDisconnected()
        {
            //context.OnDisconnected();
        }
    }
}