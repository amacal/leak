using Leak.Core.Network;
using System;

namespace Leak.Core.Loop
{
    public class ConnectionLoopConnectionToBytes : NetworkIncomingMessageHandler
    {
        private readonly ConnectionLoopConnectionContext connection;
        private readonly Action<NetworkIncomingMessage> callback;
        private readonly int bytes;

        public ConnectionLoopConnectionToBytes(ConnectionLoopConnectionContext connection, Action<NetworkIncomingMessage> callback, int bytes)
        {
            this.connection = connection;
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

        public void OnException(Exception ex)
        {
            connection.OnException(ex);
        }

        public void OnDisconnected()
        {
            connection.OnDisconnected();
        }
    }
}