using Leak.Core.Tracker;
using System;

namespace Leak.Core.Telegraph
{
    public class TrackerTelegraph
    {
        private readonly TrackerTelegraphConfiguration configuration;

        public TrackerTelegraph(Action<TrackerTelegraphConfiguration> configurer)
        {
            this.configuration = configurer.Configure(with =>
            {
                with.Callback = new TrackerTelegraphCallbackToNothing();
            });
        }

        public void Start(string tracker, Action<TrackerAnnounceConfiguration> configurer)
        {
            try
            {
                TrackerClient client = TrackerClientFactory.Create(tracker);
                configuration.Callback.OnAnnounced(client.Announce(configurer));
            }
            catch (Exception ex)
            {
                configuration.Callback.OnException(ex);
            }
        }
    }
}