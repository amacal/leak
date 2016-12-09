using Leak.Common;

namespace Leak.Events
{
    public class ConnectionEncrypted
    {
        public PeerAddress Remote;

        public NetworkConnection Connection;
    }
}