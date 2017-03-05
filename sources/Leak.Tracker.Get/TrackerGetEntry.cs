using System;
using Leak.Common;

namespace Leak.Tracker.Get
{
    public class TrackerGetEntry
    {
        public TrackerGetRegistrant Request { get; set; }
        public DateTime Next { get; set; }
    }
}