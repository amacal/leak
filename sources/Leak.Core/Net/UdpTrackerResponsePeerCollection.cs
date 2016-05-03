using System.Collections.Generic;

namespace Leak.Core.Net
{
    public class UdpTrackerResponsePeerCollection : TrackerResponsePeerCollection
    {
        private readonly byte[] data;
        private readonly int count;

        public UdpTrackerResponsePeerCollection(byte[] data, int count)
        {
            this.data = data;
            this.count = count;
        }

        public override IEnumerator<TrackerResponsePeer> GetEnumerator()
        {
            for (int i = 20; i < count; i += 6)
            {
                yield return new UdpTrackerResponsePeer(data, i);
            }
        }
    }
}