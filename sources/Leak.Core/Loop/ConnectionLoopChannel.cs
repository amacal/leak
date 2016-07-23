using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Loop
{
    public class ConnectionLoopChannel
    {
        private readonly ConnectionLoopConnection connection;
        private readonly ConnectionLoopHandshake handshake;

        public ConnectionLoopChannel(ConnectionLoopConnection connection, ConnectionLoopHandshake handshake)
        {
            this.connection = connection;
            this.handshake = handshake;
        }

        public PeerEndpoint Endpoint
        {
            get { return new PeerEndpoint(handshake.Hash, handshake.Peer, connection.Remote); }
        }

        public void Send(KeepAliveMessage message)
        {
            connection.Send(message);
        }

        public void Send(InterestedMessage message)
        {
            connection.Send(message);
        }

        public void Send(BitfieldMessage message)
        {
            connection.Send(message);
        }

        public void Send(RequestMessage message)
        {
            connection.Send(message);
        }
    }
}