using System;
using Leak.Common;

namespace Leak.Tracker.Get
{
    public class TrackerGetConfiguration
    {
        public TrackerGetConfiguration()
        {
            Peer = PeerHash.Random();
            Timeout = 30;
        }

        public PeerHash Peer;
        public int Timeout;
    }
}