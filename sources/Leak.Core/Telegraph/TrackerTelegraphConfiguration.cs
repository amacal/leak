using Leak.Core.Common;

namespace Leak.Core.Telegraph
{
    public class TrackerTelegraphConfiguration
    {
        public int Port { get; set; }

        public PeerHash Peer { get; set; }

        public TrackerTelegraphCallback Callback { get; set; }
    }
}