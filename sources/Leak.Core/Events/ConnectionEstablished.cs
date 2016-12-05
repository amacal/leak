using Leak.Common;
using Leak.Core.Network;

namespace Leak.Core.Events
{
    public class ConnectionEstablished
    {
        public PeerAddress Remote;

        public NetworkConnection Connection;
    }
}