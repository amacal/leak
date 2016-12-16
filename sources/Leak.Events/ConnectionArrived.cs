using Leak.Common;

namespace Leak.Events
{
    public class ConnectionArrived
    {
        public PeerAddress Remote;

        public NetworkConnection Connection;
    }
}