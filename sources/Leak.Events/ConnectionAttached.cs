using Leak.Common;

namespace Leak.Events
{
    public class ConnectionAttached
    {
        public PeerAddress Remote;

        public NetworkConnection Connection;
    }
}