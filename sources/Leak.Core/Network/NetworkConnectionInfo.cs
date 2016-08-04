namespace Leak.Core.Network
{
    public class NetworkConnectionInfo
    {
        private readonly string remote;
        private readonly NetworkDirection direction;

        public NetworkConnectionInfo(string remote, NetworkDirection direction)
        {
            this.remote = remote;
            this.direction = direction;
        }

        public string Remote
        {
            get { return remote; }
        }

        public NetworkDirection Direction
        {
            get { return direction; }
        }
    }
}