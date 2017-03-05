using System;

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

        public void Announce(TrackerGetRegistrant request, Action<TimeSpan> callback)
        {
            context.Udp.Register(new TrackerGetUdpRegistrant
            {
                Host = host,
                Port = port,
                Request = request,
                Callback = callback
            });
        }
    }
}