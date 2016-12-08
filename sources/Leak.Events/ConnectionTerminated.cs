using Leak.Common;

namespace Leak.Events
{
    public class ConnectionTerminated
    {
        public PeerAddress Remote;

        public NetworkConnection Connection;
    }
}