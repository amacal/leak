using System;

namespace Leak.Core.Net
{
    public abstract class TrackerResonse
    {
        public abstract TimeSpan Interval { get; }

        public abstract TrackerResponsePeerCollection Peers { get; }
    }
}