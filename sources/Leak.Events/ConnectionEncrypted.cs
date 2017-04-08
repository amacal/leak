using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Events
{
    public class ConnectionEncrypted
    {
        public NetworkAddress Remote;

        public NetworkConnection Connection;
    }
}