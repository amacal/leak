using Leak.Common;
using Leak.Core.Network;

namespace Leak.Core.Events
{
    public class ConnectionTerminated
    {
        public PeerAddress Remote;

        public NetworkConnection Connection;
    }
}