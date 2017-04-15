using Leak.Common;

namespace Leak.Peer.Coordinator.Events
{
    public class PeerStatusChanged
    {
        public PeerHash Peer;
        public PeerState State;
    }
}