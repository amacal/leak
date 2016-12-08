using Leak.Common;

namespace Leak.Events
{
    public class ConnectionEstablished
    {
        public PeerAddress Remote;

        public NetworkConnection Connection;
    }
}