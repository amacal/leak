using Leak.Common;

namespace Leak.Events
{
    public class ConnectionReceived
    {
        public PeerAddress Remote;

        public NetworkConnection Connection;

        public int Bytes;
    }
}