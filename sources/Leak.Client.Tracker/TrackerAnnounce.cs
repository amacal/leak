using System;
using Leak.Common;

namespace Leak.Client.Tracker
{
    public class TrackerAnnounce
    {
        public FileHash Hash { get; set; }

        public int? Leechers { get; set; }
        public int? Seeders { get; set; }

        public TimeSpan Interval { get; set; }
        public PeerAddress[] Peers { get; set; }
    }
}