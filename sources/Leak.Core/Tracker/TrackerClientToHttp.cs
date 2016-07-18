using System;

namespace Leak.Core.Tracker
{
    public class TrackerClientToHttp : TrackerClient
    {
        private readonly string uri;

        public TrackerClientToHttp(string uri)
        {
            this.uri = uri;
        }

        public TrackerAnnounce Announce(Action<TrackerAnnounceConfiguration> configurer)
        {
            throw new NotImplementedException();
        }
    }
}