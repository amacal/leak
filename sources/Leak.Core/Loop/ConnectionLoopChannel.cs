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

        public string Remote
        {
            get { return connection.Remote; }
        }

        public PeerHash Peer
        {
            get { return handshake.Peer; }
        }

        public FileHash Hash
        {
            get { return handshake.Hash; }
        }

        public void Send(InterestedMessage message)
        {
            connection.Send(message);
        }

        public void Send(RequestMessage message)
        {
            connection.Send(message);
        }
    }
}