using System.Collections.Generic;
using Leak.Core.Bencoding;

namespace Leak.Core.Net
{
    public class HttpTrackerResponsePeerCollection : TrackerResponsePeerCollection
    {
        private readonly BencodedValue data;

        public HttpTrackerResponsePeerCollection(BencodedValue data)
        {
            this.data = data;
        }

        public override IEnumerator<TrackerResponsePeer> GetEnumerator()
        {
            byte[] bytes = data.Data.GetBytes();

            for (int i = 0; i < bytes.Length; i += 6)
            {
                if (bytes[i + 4] > 0 && bytes[i + 5] > 0)
                {
                    yield return new HttpTrackerResponsePeer(bytes, i);
                }
            }
        }
    }
}