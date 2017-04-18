using Leak.Common;

namespace Leak.Peer.Coordinator.Events
{
    public class StatusChanged
    {
        public PeerHash Peer;
        public PeerState State;
    }
}