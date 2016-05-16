using System;

namespace Leak.Core.Net
{
    public class UdpTrackerResponse : TrackerResonse
    {
        private readonly byte[] data;
        private readonly int count;

        public UdpTrackerResponse(byte[] data, int count)
        {
            this.data = data;
            this.count = count;
        }

        public override TimeSpan Interval
        {
            get { return TimeSpan.FromSeconds(data[10] * 256 + data[11]); }
        }

        public override TrackerResponsePeerCollection Peers
        {
            get { return new UdpTrackerResponsePeerCollection(data, count); }
        }
    }
}