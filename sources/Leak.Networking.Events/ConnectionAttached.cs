using Leak.Networking.Core;

namespace Leak.Networking.Events
{
    public class ConnectionAttached
    {
        public NetworkAddress Remote;

        public NetworkConnection Connection;
    }
}