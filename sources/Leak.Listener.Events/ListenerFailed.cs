using Leak.Common;

namespace Leak.Listener.Events
{
    public class ListenerFailed
    {
        public PeerHash Peer;
        public string Reason;
    }
}