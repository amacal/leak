using Leak.Core.IO;
using System;

namespace Leak.Core.Net
{
    public static class TrackerClientFactory
    {
        public static bool IsSupported(MetainfoTracker tracker)
        {
            return tracker.Protocol == MetainfoTrackerProtocol.Http
                || tracker.Protocol == MetainfoTrackerProtocol.Udp;
        }

        public static TrackerClient Create(string tracker)
        {
            return Create(new MetainfoTracker(new Uri(tracker)));
        }

        public static TrackerClient Create(MetainfoTracker tracker)
        {
            switch (tracker.Protocol)
            {
                case MetainfoTrackerProtocol.Http:
                    return new HttpTrackerClient(tracker.Uri);

                case MetainfoTrackerProtocol.Udp:
                    return new UdpTrackerClient(tracker.Uri.Host, tracker.Uri.Port);

                default:
                    throw new NotSupportedException();
            }
        }
    }
}