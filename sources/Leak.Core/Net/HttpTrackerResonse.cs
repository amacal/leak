using Leak.Core.Encoding;
using System;

namespace Leak.Core.Net
{
    public class HttpTrackerResonse : TrackerResonse
    {
        private readonly BencodedValue data;

        public HttpTrackerResonse(byte[] data)
        {
            this.data = Bencoder.Decode(data);
        }

        public override TimeSpan Interval
        {
            get
            {
                return data.Find("interval", x => TimeSpan.FromSeconds(x.ToNumber()));
            }
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