using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Events
{
    public class PeerDisconnected
    {
        public PeerHash Peer;
        public NetworkAddress Remote;
    }
}