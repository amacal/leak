using Leak.Common;
using System;

namespace Leak.Core.Loop
{
    public class ConnectionLoopConnectionToBytes : NetworkIncomingMessageHandler
    {
        private readonly Action<NetworkIncomingMessage> callback;
        private readonly int bytes;

        public ConnectionLoopConnectionToBytes(Action<NetworkIncomingMessage> callback, int bytes)
        {
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
        }
    }
}