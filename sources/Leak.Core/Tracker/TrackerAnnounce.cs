using Leak.Common;
using System;

namespace Leak.Core.Tracker
{
    public class TrackerAnnounce
    {
        private readonly PeerHash peer;
        private readonly FileHash hash;
        private readonly PeerAddress[] peers;
        private readonly TimeSpan interval;

        public TrackerAnnounce(PeerHash peer, FileHash hash, PeerAddress[] peers, TimeSpan interval)
        {
            this.peer = peer;
            this.hash = hash;
            this.peers = peers;
            this.interval = interval;
        }

        public PeerHash Peer
        {
            get { return peer; }
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