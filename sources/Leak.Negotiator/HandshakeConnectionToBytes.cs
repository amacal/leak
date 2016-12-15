using Leak.Common;
using System;

namespace Leak.Negotiator
{
    public class HandshakeConnectionToBytes : NetworkIncomingMessageHandler
    {
        private readonly HandshakeNegotiatorHooks hooks;
        private readonly Action<NetworkIncomingMessage> callback;
        private readonly int bytes;

        public HandshakeConnectionToBytes(HandshakeNegotiatorHooks hooks, Action<NetworkIncomingMessage> callback, int bytes)
        {
            this.hooks = hooks;
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
            //context.OnDisconnected();
        }
    }
}