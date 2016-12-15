using Leak.Common;

namespace Leak.Events
{
    public class HandshakeRejected
    {
        public FileHash Hash;

        public NetworkConnection Connection;
    }
}