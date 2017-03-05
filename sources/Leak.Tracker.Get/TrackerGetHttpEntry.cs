using System;
using System.Net;
using Leak.Common;
using Leak.Sockets;

namespace Leak.Tracker.Get
{
    public class TrackerGetHttpEntry
    {
        public FileHash Hash { get; set; }
        public IPEndPoint Endpoint { get; set; }
        public Uri Address { get; set; }
        public DateTime Deadline { get; set; }
        public TcpSocket Socket { get; set; }
        public SocketBuffer Buffer { get; set; }
        public TrackerGetHttpStatus Status { get; set; }
        public Action<TimeSpan> Callback { get; set; }
    }
}