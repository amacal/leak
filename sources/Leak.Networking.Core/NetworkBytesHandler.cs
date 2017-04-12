using System;

namespace Leak.Networking.Core
{
    public class NetworkBytesHandler : NetworkIncomingMessageHandler
    {
        private readonly Action<NetworkIncomingMessage> callback;
        private readonly int bytes;

        public NetworkBytesHandler(Action<NetworkIncomingMessage> callback, int bytes)
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