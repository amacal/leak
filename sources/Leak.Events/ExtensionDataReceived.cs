using Leak.Common;

namespace Leak.Events
{
    public class ExtensionDataReceived
    {
        public PeerHash Peer;
        public string Extension;
        public int Size;
    }
}