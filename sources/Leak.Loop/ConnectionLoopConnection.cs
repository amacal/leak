using System;
using Leak.Networking.Core;

namespace Leak.Peer.Receiver
{
    public class ConnectionLoopConnection
    {
        private readonly NetworkConnection connection;

        public ConnectionLoopConnection(NetworkConnection connection)
        {
            this.connection = connection;
        }

        public void Receive(Action<NetworkIncomingMessage> callback, int bytes)
        {
            connection.ReceiveIf(callback, bytes);
        }
    }
}