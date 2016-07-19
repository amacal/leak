using System;

namespace Leak.Core.Tracker
{
    public class TrackerClientFactory
    {
        public static TrackerClient Create(string uri)
        {
            if (uri.StartsWith("http://"))
                return new TrackerClientToHttp(uri);

            if (uri.StartsWith("udp://"))
                return new TrackerClientToUdp();

            throw new NotSupportedException("The tracker type is not supported.");
        }
    }
}