using Leak.Common;

namespace Leak.Core.Telegraph.Events
{
    public class TrackerAnnounceFailed
    {
        public FileHash Hash;

        public PeerHash Peer;

        public string Tracker;

        public string Reason;
    }
}