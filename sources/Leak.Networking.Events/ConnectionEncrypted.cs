using Leak.Networking.Core;

namespace Leak.Networking.Events
{
    public class ConnectionEncrypted
    {
        public NetworkAddress Remote;

        public NetworkConnection Connection;
    }
}