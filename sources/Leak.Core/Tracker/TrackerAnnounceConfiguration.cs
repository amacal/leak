using Leak.Core.Common;

namespace Leak.Core.Tracker
{
    public class TrackerAnnounceConfiguration
    {
        public FileHash Hash { get; set; }

        public PeerHash Peer { get; set; }
    }
}