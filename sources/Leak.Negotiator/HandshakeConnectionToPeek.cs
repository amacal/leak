using Leak.Common;
using System;
using Leak.Networking.Core;

namespace Leak.Negotiator
{
    public class HandshakeConnectionToPeek : NetworkIncomingMessageHandler
    {
        private readonly HandshakeConnection connection;
        private readonly Action<NetworkIncomingMessage> callback;
        private readonly Func<NetworkIncomingMessage, bool> peek;

        public HandshakeConnectionToPeek(HandshakeConnection connection, Action<NetworkIncomingMessage> callback, Func<NetworkIncomingMessage, bool> peek)
        {
            this.connection = connection;
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
            connection.CallHandshakeRejected();
        }
    }
}