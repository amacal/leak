using Leak.Core.Tracker;
using System;

namespace Leak.Core.Telegraph
{
    public class TrackerTelegraphRequest
    {
        private readonly TrackerClient client;
        private readonly Action<TrackerAnnounceConfiguration> request;
        private readonly TrackerStorageEntry entry;

        public TrackerTelegraphRequest(TrackerClient client, Action<TrackerAnnounceConfiguration> request, TrackerStorageEntry entry)
        {
            this.client = client;
            this.request = request;
            this.entry = entry;
        }

        public void Execute(TrackerTelegraphCallback callback)
        {
            entry.Trigger = DateTime.Now.AddMinutes(30);

            try
            {
                TrackerAnnounce response = client.Announce(request);

                callback.OnAnnounced(response);
                entry.Trigger = DateTime.Now.AddMinutes(30);
            }
            catch (Exception ex)
            {
                callback.OnException(ex);
            }
        }
    }
}