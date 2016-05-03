using System.Collections;
using System.Collections.Generic;

namespace Leak.Core.Net
{
    public abstract class TrackerResponsePeerCollection : IEnumerable<TrackerResponsePeer>
    {
        public abstract IEnumerator<TrackerResponsePeer> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}