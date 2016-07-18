using System;

namespace Leak.Core.Tracker
{
    public class TrackerClientToUdp : TrackerClient
    {
        public TrackerAnnounce Announce(Action<TrackerAnnounceConfiguration> configurer)
        {
            throw new NotImplementedException();
        }
    }
}