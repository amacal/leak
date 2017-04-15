using Leak.Common;

namespace Leak.Peer.Coordinator.Events
{
    public class ExtensionListReceived
    {
        public PeerHash Peer;

        public string[] Extensions;
    }
}