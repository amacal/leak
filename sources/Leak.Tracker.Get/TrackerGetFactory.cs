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
                    return new TrackerGetUdpTracker(context, address.Host, address.Port);
            }

            return null;
        }
    }
}