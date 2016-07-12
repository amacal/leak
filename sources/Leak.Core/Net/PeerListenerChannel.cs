using Leak.Core.Network;

namespace Leak.Core.Net
{
    public class PeerChannelImpl : PeerChannel
    {
        private readonly NetworkConnection connection;
        private readonly PeerCallback callback;
        private readonly byte[] hash;

        public PeerChannelImpl(NetworkConnection connection, PeerCallback callback, byte[] hash)
        {
            this.connection = connection;
            this.callback = callback;
            this.hash = hash;
        }

        public byte[] Hash
        {
            get { return hash; }
        }

        public string Name
        {
            get { return connection.Remote; }
        }

        public NetworkConnectionDirection Direction
        {
            get { return connection.Direction; }
        }

        public void Send(PeerMessageFactory data)
        {
            connection.Send(data.GetMessage());
        }

        public void Start(PeerMessageLoop loop)
        {
            connection.Receive(loop);
        }
    }
}