using Leak.Networking.Core;

namespace Leak.Networking.Events
{
    public class ConnectionSent
    {
        public NetworkAddress Remote;

        public NetworkConnection Connection;

        public int Bytes;
    }
}