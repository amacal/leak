using System;

namespace Leak.Tracker.Get
{
    public class TrackerGetHttpProxy : TrackerGetProxy
    {
        private readonly TrackerGetContext context;
        private readonly Uri address;

        public TrackerGetHttpProxy(TrackerGetContext context, Uri address)
        {
            this.context = context;
            this.address = address;
        }

        public void Announce(TrackerGetRegistrant request, Action<TimeSpan> callback)
        {
            context.Http.Register(new TrackerGetHttpRegistrant
            {
                Address = address,
                Request = request,
                Callback = callback
            });
        }
    }
}