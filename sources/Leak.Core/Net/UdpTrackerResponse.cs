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

        public override TrackerResponsePeerCollection Peers
        {
            get { return new UdpTrackerResponsePeerCollection(data, count); }
        }
    }
}