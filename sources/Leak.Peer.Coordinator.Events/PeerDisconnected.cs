using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Coordinator.Events
{
    public class PeerDisconnected
    {
        public PeerHash Peer;
        public NetworkAddress Remote;
    }
}