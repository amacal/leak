using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Events
{
    public class ConnectionTerminated
    {
        public NetworkAddress Remote;

        public NetworkConnection Connection;
    }
}