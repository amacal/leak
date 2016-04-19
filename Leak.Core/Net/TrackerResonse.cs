using Leak.Core.Encoding;

namespace Leak.Core.Net
{
    public class TrackerResonse
    {
        private readonly BencodedValue data;

        public TrackerResonse(byte[] data)
        {
            this.data = Bencoder.Decode(data);
        }

        public TrackerResponsePeerCollection Peers
        {
            get
            {
                return data.Find("peers", x => new TrackerResponsePeerCollection(x));
            }
        }
    }
}