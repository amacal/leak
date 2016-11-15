using Leak.Core.Common;
using Leak.Core.Network;

namespace Leak.Core.Events
{
    public class ConnectionDropped
    {
        public PeerAddress Remote;

        public NetworkConnection Connection;

        public string Reason;
    }
}