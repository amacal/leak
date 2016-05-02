using Leak.Core.Encoding;
using System.Collections;
using System.Collections.Generic;

namespace Leak.Core.Net
{
    public class TrackerResponsePeerCollection : IEnumerable<TrackerResponsePeer>
    {
        private readonly BencodedValue data;

        public TrackerResponsePeerCollection(BencodedValue data)
        {
            this.data = data;
        }

        public IEnumerator<TrackerResponsePeer> GetEnumerator()
        {
            byte[] bytes = data.ToHex();

            for (int i = 0; i < bytes.Length; i += 6)
            {
                if (bytes[i + 4] > 0 && bytes[i + 5] > 0)
                {
                    yield return new TrackerResponsePeer(bytes, i);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}