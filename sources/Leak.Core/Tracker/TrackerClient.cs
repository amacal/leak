using System;

namespace Leak.Core.Tracker
{
    public interface TrackerClient
    {
        TrackerAnnounce Announce(Action<TrackerAnnounceConfiguration> configurer);
    }
}