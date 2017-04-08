using System;
using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Tracker.Get.Events
{
    public class TrackerAnnounced
    {
        public FileHash Hash;
        public PeerHash Peer;

        public Uri Address;
        public NetworkAddress[] Peers;
        public TimeSpan Interval;

        public int? Seeders;
        public int? Leechers;
    }
}