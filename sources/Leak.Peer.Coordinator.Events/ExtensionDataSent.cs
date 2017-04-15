using Leak.Common;

namespace Leak.Peer.Coordinator.Events
{
    public class ExtensionDataSent
    {
        public PeerHash Peer;
        public string Extension;
        public int Size;
    }
}