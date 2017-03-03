using System;
using Leak.Common;

namespace Leak.Tracker.Get.Events
{
    public class TrackerTimeout
    {
        public FileHash Hash;
        public PeerHash Peer;

        public Uri Address;
        public int Seconds;
    }
}