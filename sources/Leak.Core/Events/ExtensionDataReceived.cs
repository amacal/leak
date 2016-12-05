using Leak.Common;

namespace Leak.Core.Events
{
    public class ExtensionDataReceived
    {
        public PeerHash Peer;

        public string Extension;

        public int Size;
    }
}