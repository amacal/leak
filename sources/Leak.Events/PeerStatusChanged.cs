using Leak.Common;

namespace Leak.Events
{
    public class PeerStatusChanged
    {
        public PeerHash Peer;
        public PeerState State;
    }
}