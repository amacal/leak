using Leak.Networking.Core;

namespace Leak.Networking.Events
{
    public class ConnectionReceived
    {
        public NetworkAddress Remote;

        public NetworkConnection Connection;

        public int Bytes;
    }
}