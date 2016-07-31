using Leak.Core.Network;
using System;

namespace Leak.Core.Loop
{
    public class ConnectionLoopConnection
    {
        private readonly ConnectionLoopConfiguration configuration;
        private readonly NetworkConnection connection;
        private readonly ConnectionLoopHandshake handshake;

        public ConnectionLoopConnection(ConnectionLoopConfiguration configuration, NetworkConnection connection, ConnectionLoopHandshake handshake)
        {
            this.configuration = configuration;
            this.connection = connection;
            this.handshake = handshake;
        }

        public string Remote
        {
            get { return connection.Remote; }
        }

        public NetworkDirection Direction
        {
            get { return connection.Direction; }
        }

        public void Receive(Action<NetworkIncomingMessage> callback, int bytes)
        {
            ConnectionLoopConnectionContext context = new ConnectionLoopConnectionContext(configuration, this, handshake);
            ConnectionLoopConnectionToBytes handler = new ConnectionLoopConnectionToBytes(context, callback, bytes);

            connection.Receive(handler);
        }

        public void Send(NetworkOutgoingMessage message)
        {
            connection.Send(message);
        }
    }
}