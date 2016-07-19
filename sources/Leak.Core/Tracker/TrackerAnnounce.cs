namespace Leak.Core.Tracker
{
    public class TrackerAnnounce
    {
        private readonly TrackerPeer[] peers;

        public TrackerAnnounce(TrackerPeer[] peers)
        {
            this.peers = peers;
        }

        public TrackerPeer[] Peers
        {
            get { return peers; }
        }
    }
}