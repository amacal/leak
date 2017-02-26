using System;
using Leak.Common;

namespace Leak.Events
{
    public class TrackerFailed
    {
        public FileHash Hash;
        public PeerHash Peer;

        public Uri Address;
        public string Reason;
    }
}