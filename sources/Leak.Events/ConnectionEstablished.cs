using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Events
{
    public class ConnectionEstablished
    {
        public NetworkAddress Remote;

        public NetworkConnection Connection;
    }
}