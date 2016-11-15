using Leak.Core.Common;

namespace Leak.Core.Events
{
    public class ExtensionListReceived
    {
        public PeerHash Peer;

        public string[] Extensions;
    }
}