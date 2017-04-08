using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Events
{
    public class ConnectionSent
    {
        public NetworkAddress Remote;

        public NetworkConnection Connection;

        public int Bytes;
    }
}