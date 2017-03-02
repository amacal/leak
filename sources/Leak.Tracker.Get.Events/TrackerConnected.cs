using System;
using Leak.Common;

namespace Leak.Tracker.Get.Events
{
    public class TrackerConnected
    {
        public FileHash Hash;
        public PeerHash Peer;

        public Uri Address;

        public byte[] Transaction;
        public byte[] Connection;
    }
}