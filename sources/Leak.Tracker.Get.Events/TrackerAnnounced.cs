using System;
using Leak.Common;

namespace Leak.Tracker.Get.Events
{
    public class TrackerAnnounced
    {
        public FileHash Hash;
        public PeerHash Peer;

        public Uri Address;
        public PeerAddress[] Peers;
        public TimeSpan Interval;

        public int? Seeders;
        public int? Leechers;
    }
}