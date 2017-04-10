using Leak.Networking.Core;

namespace Leak.Listener.Events
{
    public class ConnectionArrived
    {
        public NetworkAddress Remote;

        public NetworkConnection Connection;
    }
}