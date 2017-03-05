using System;

namespace Leak.Tracker.Get
{
    public class TrackerGetUdpRegistrant
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public Action<TimeSpan> Callback { get; set; }
        public TrackerGetRegistrant Request { get; set; }
    }
}