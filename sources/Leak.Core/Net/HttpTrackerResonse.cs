using Leak.Core.Encoding;

namespace Leak.Core.Net
{
    public class HttpTrackerResonse : TrackerResonse
    {
        private readonly BencodedValue data;

        public HttpTrackerResonse(byte[] data)
        {
            this.data = Bencoder.Decode(data);
        }

        public override TrackerResponsePeerCollection Peers
        {
            get
            {
                return data.Find("peers", x => new HttpTrackerResponsePeerCollection(x));
            }
        }
    }
}