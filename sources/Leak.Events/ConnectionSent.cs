using Leak.Common;

namespace Leak.Events
{
    public class ConnectionSent
    {
        public PeerAddress Remote;

        public NetworkConnection Connection;

        public int Bytes;
    }
}