using Leak.Core.Common;
using System;

namespace Leak.Core.Telegraph.Events
{
    public class TrackerAnnounceCompleted
    {
        public FileHash Hash;

        public PeerHash Peer;

        public string Tracker;

        public PeerAddress[] Peers;

        public TimeSpan Interval;
    }
}