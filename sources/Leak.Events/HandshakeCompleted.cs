using Leak.Common;

namespace Leak.Events
{
    public class HandshakeCompleted
    {
        public FileHash Hash;

        public NetworkConnection Connection;

        public Handshake Handshake;
    }
}