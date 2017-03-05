using System;

namespace Leak.Tracker.Get
{
    public class TrackerGetFactory
    {
        private readonly TrackerGetContext context;

        public TrackerGetFactory(TrackerGetContext context)
        {
            this.context = context;
        }

        public TrackerGetProxy Create(Uri address)
        {
            switch (address.Scheme)
            {
                case "udp":
                    return new TrackerGetUdpProxy(context, address.Host, address.Port);

                case "http":
                    return new TrackerGetHttpProxy(context, address);
            }

            return null;
        }
    }
}