using System;
using System.Net;

namespace Leak.Tracker.Get
{
    public class TrackerGetUdpEntry
    {
        public TrackerGetRegistrant Request { get; set; }
        public IPEndPoint Endpoint { get; set; }
        public Uri Address { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public byte[] Transaction { get; set; }
        public byte[] Connection { get; set; }
        public DateTime Deadline { get; set; }
        public TrackerGetUdpStatus Status { get; set; }
        public Action<TimeSpan> Callback { get; set; }
    }
}