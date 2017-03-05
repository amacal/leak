using System;
using Leak.Common;

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

        public void Announce(FileHash hash, Action<TimeSpan> callback)
        {
            context.Http.Register(new TrackerGetHttpRegistrant
            {
                Hash = hash,
                Address = address,
                Callback = callback
            });
        }
    }
}