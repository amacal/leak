using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Leak.Common;
using Leak.Sockets;
using Leak.Tasks;

namespace Leak.Tracker.Get
{
    public class TrackerGetUdpTracker : TrackerGetProxy
    {
        private readonly TrackerGetContext context;
        private readonly string host;
        private readonly int port;

        public TrackerGetUdpTracker(TrackerGetContext context, string host, int port)
        {
            this.context = context;
            this.host = host;
            this.port = port;
        }

        public void Announce(FileHash hash)
        {
            TrackerGetUdpRegistrant registrant = new TrackerGetUdpRegistrant
            {
                Hash = hash,
                Host = host,
                Port = port
            };

            context.UdpService.Register(Bytes.Random(4), registrant);
        }
    }
}