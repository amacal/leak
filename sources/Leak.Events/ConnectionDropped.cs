using Leak.Common;

namespace Leak.Events
{
    public class ConnectionDropped
    {
        public PeerAddress Remote;

        public NetworkConnection Connection;

        public string Reason;
    }
}