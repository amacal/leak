using Leak.Core.Common;
using Leak.Core.Network;

namespace Leak.Core.Events
{
    public class ConnectionAttached
    {
        public PeerAddress Remote;

        public NetworkConnection Connection;
    }
}