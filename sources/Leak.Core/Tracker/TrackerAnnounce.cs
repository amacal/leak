using Leak.Core.Common;
using System;

namespace Leak.Core.Tracker
{
    public class TrackerAnnounce
    {
        private readonly FileHash hash;
        private readonly PeerAddress[] peers;
        private readonly TimeSpan interval;

        public TrackerAnnounce(FileHash hash, PeerAddress[] peers, TimeSpan interval)
        {
            this.hash = hash;
            this.peers = peers;
            this.interval = interval;
        }

        public FileHash Hash
        {
            get { return hash; }
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