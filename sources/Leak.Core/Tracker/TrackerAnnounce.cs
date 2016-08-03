using Leak.Core.Common;
using System;

namespace Leak.Core.Tracker
{
    public class TrackerAnnounce
    {
        private readonly PeerAddress[] peers;
        private readonly TimeSpan interval;

        public TrackerAnnounce(PeerAddress[] peers, TimeSpan interval)
        {
            this.peers = peers;
            this.interval = interval;
        }

        public PeerAddress[] Peers
        {
            get { return peers; }
        }

        public TimeSpan Interval
        {
            get { return interval; }
        }
    }
}