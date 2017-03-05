using System;
using Leak.Common;

namespace Leak.Tracker.Get
{
    public class TrackerGetHttpRegistrant
    {
        public FileHash Hash { get; set; }
        public Uri Address { get; set; }
        public Action<TimeSpan> Callback { get; set; }
    }
}