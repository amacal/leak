using Leak.Common;

namespace Leak.Core.Connector
{
    public class PeerConnectorConnected
    {
        private readonly FileHash hash;
        private readonly NetworkConnection connection;

        public PeerConnectorConnected(FileHash hash, NetworkConnection connection)
        {
            this.hash = hash;
            this.connection = connection;
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public NetworkConnection Connection
        {
            get { return connection; }
        }
    }
}