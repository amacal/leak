using System;
using Leak.Common;

namespace Leak.Tracker.Get
{
    public class TrackerGetUdpProxy : TrackerGetProxy
    {
        private readonly TrackerGetContext context;
        private readonly string host;
        private readonly int port;

        public TrackerGetUdpProxy(TrackerGetContext context, string host, int port)
        {
            this.context = context;
            this.host = host;
            this.port = port;
        }

        public void Announce(FileHash hash, Action<TimeSpan> callback)
        {
            context.Udp.Register(new TrackerGetUdpRegistrant
            {
                Hash = hash,
                Host = host,
                Port = port,
                Callback = callback
            });
        }
    }
}