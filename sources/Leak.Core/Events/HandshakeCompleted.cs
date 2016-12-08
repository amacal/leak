using Leak.Common;
using Leak.Core.Negotiator;
using Leak.Core.Network;

namespace Leak.Core.Events
{
    public class HandshakeCompleted
    {
        public NetworkConnection Connection;

        public Handshake Handshake;
    }
}