using Leak.Common;

namespace Leak.Events
{
    public class ExtensionListSent
    {
        public PeerHash Peer;
        public string[] Extensions;
    }
}