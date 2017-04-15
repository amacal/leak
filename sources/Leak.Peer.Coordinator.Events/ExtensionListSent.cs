using Leak.Common;

namespace Leak.Peer.Coordinator.Events
{
    public class ExtensionListSent
    {
        public PeerHash Peer;
        public string[] Extensions;
    }
}