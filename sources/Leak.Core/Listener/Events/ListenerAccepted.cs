using Leak.Core.Common;
using Leak.Core.Network;

namespace Leak.Core.Listener.Events
{
    public class ListenerAccepted
    {
        public PeerHash Local;

        public PeerAddress Remote;

        public NetworkConnection Connection;
    }
}