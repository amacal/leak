using Leak.Core.Tracker;
using System;
using System.Threading;

namespace Leak.Core.Telegraph
{
    public class TrackerTelegraph
    {
        private readonly TrackerTelegraphConfiguration configuration;
        private readonly TrackerStorage storage;

        private readonly Timer timer;

        public TrackerTelegraph(Action<TrackerTelegraphConfiguration> configurer)
        {
            this.configuration = configurer.Configure(with =>
            {
                with.Callback = new TrackerTelegraphCallbackToNothing();
            });

            this.storage = new TrackerStorage();

            timer = new Timer(OnTick);
            timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(10));
        }

        private void OnTick(object state)
        {
            foreach (TrackerTelegraphRequest request in storage.GetRequests())
            {
                request.Execute(configuration.Callback);
            }
        }

        public void Start(string tracker, Action<TrackerAnnounceConfiguration> configurer)
        {
            storage.Register(tracker, configurer);
        }
    }
}