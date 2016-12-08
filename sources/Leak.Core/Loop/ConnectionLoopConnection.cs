using System;
using Leak.Common;

namespace Leak.Core.Loop
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
            connection.Receive(new ConnectionLoopConnectionToBytes(callback, bytes));
        }
    }
}