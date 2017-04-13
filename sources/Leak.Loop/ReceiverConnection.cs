using System;
using Leak.Networking.Core;

namespace Leak.Peer.Receiver
{
    public class ReceiverConnection
    {
        private readonly NetworkConnection connection;

        public ReceiverConnection(NetworkConnection connection)
        {
            this.connection = connection;
        }

        public void Receive(Action<NetworkIncomingMessage> callback, int bytes)
        {
            connection.ReceiveIf(callback, bytes);
        }
    }
}