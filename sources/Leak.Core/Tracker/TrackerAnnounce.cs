using System;

namespace Leak.Core.Tracker
{
    public class TrackerAnnounce
    {
        private readonly TrackerPeer[] peers;
        private readonly TimeSpan interval;

        public TrackerAnnounce(TrackerPeer[] peers, TimeSpan interval)
        {
            this.peers = peers;
            this.interval = interval;
        }

        public TrackerPeer[] Peers
        {
            get { return peers; }
        }

        public TimeSpan Interval
        {
            get { return interval; }
        }
    }
}