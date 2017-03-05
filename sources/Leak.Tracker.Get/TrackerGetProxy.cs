using System;

namespace Leak.Tracker.Get
{
    public interface TrackerGetProxy
    {
        void Announce(TrackerGetRegistrant request, Action<TimeSpan> callback);
    }
}