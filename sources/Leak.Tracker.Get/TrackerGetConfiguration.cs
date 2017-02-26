using System;
using Leak.Common;

namespace Leak.Tracker.Get
{
    public class TrackerGetConfiguration
    {
        public TrackerGetConfiguration()
        {
            Peer = PeerHash.Random();
            Timeout = TimeSpan.FromSeconds(30);
        }

        public PeerHash Peer;
        public TimeSpan Timeout;
    }
}