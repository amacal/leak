using System;
using System.Net;
using Leak.Common;

namespace Leak.Tracker.Get.Events
{
    public class TrackerPacketSent
    {
        public FileHash Hash;
        public PeerHash Peer;

        public Uri Address;
        public IPEndPoint Endpoint;

        public int Size;
    }
}