using Leak.Networking.Core;

namespace Leak.Networking.Events
{
    public class ConnectionTerminated
    {
        public NetworkAddress Remote;

        public NetworkConnection Connection;
    }
}