using System;

namespace Leak.Core.Loop
{
    public class ConnectionLoopConnectionContext
    {
        private readonly ConnectionLoopConfiguration configuration;
        private readonly ConnectionLoopConnection connection;
        private readonly ConnectionLoopHandshake handshake;

        public ConnectionLoopConnectionContext(ConnectionLoopConfiguration configuration, ConnectionLoopConnection connection, ConnectionLoopHandshake handshake)
        {
            this.configuration = configuration;
            this.connection = connection;
            this.handshake = handshake;
        }

        public void OnException(Exception ex)
        {
            configuration.Callback.OnException(new ConnectionLoopChannel(connection, handshake), ex);
        }

        public void OnDisconnected()
        {
            configuration.Callback.OnDisconnected(new ConnectionLoopChannel(connection, handshake));
        }
    }
}