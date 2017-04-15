using Leak.Common;

namespace Leak.Peer.Coordinator.Events
{
    public class ExtensionDataReceived
    {
        public PeerHash Peer;
        public string Extension;
        public int Size;
    }
}