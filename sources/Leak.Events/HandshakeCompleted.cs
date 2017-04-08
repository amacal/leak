using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Events
{
    public class HandshakeCompleted
    {
        public FileHash Hash;

        public NetworkConnection Connection;

        public Handshake Handshake;
    }
}