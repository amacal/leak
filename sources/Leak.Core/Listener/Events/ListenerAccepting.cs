using Leak.Core.Common;

namespace Leak.Core.Listener.Events
{
    public class ListenerAccepting
    {
        public PeerHash Local;

        public PeerAddress Remote;
    }
}