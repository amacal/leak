using Leak.Common;

namespace Leak.Core.Network
{
    public class NetworkConnectionInfo
    {
        private readonly PeerAddress remote;
        private readonly NetworkDirection direction;

        public NetworkConnectionInfo(PeerAddress remote, NetworkDirection direction)
        {
            this.remote = remote;
            this.direction = direction;
        }

        public PeerAddress Remote
        {
            get { return remote; }
        }

        public NetworkDirection Direction
        {
            get { return direction; }
        }
    }
}