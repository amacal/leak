using Leak.Common;

namespace Leak.Core.Telegraph.Events
{
    public class TrackerAnnounceStarted
    {
        public FileHash Hash;

        public PeerHash Peer;

        public string Tracker;
    }
}