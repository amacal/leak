using Leak.Common;

namespace Leak.Events
{
    public class PeerDisconnected
    {
        public PeerHash Peer;
        public PeerAddress Remote;
    }
}