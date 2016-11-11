using Leak.Core.Common;

namespace Leak.Core.Listener.Events
{
    public class ListenerStarted
    {
        public PeerHash Peer;

        public int Port;
    }
}