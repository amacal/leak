using System;

namespace Leak.Tracker.Get
{
    public class TrackerGetHttpRegistrant
    {
        public Uri Address { get; set; }
        public Action<TimeSpan> Callback { get; set; }
        public TrackerGetRegistrant Request { get; set; }
    }
}