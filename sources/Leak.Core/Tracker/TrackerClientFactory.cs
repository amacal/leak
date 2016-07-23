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
                return CreateUdpClient(uri);

            throw new NotSupportedException("The tracker type is not supported.");
        }

        private static TrackerClient CreateUdpClient(string uri)
        {
            Uri link = new Uri(uri, UriKind.Absolute);
            TrackerClientToUdp client = new TrackerClientToUdp(link.Host, link.Port);

            return client;
        }
    }
}