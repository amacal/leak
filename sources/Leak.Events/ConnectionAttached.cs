using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Events
{
    public class ConnectionAttached
    {
        public NetworkAddress Remote;

        public NetworkConnection Connection;
    }
}