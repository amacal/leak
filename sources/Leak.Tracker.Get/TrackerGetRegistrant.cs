using System;
using Leak.Common;

namespace Leak.Tracker.Get
{
    public class TrackerGetRegistrant
    {
        public Uri Address { get; set; }
        public PeerHash Peer { get; set; }
        public FileHash Hash { get; set; }
        public int? Port { get; set; }
        public TrackerGetProgress Progress { get; set; }
        public TrackerGetEvent? Event { get; set; }
    }
}