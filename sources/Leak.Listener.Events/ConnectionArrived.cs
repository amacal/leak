using Leak.Common;

namespace Leak.Listener.Events
{
    public class ConnectionArrived
    {
        public PeerAddress Remote;

        public NetworkConnection Connection;
    }
}