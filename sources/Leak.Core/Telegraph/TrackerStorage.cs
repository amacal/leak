using Leak.Core.Tracker;
using System;
using System.Collections.Generic;

namespace Leak.Core.Telegraph
{
    public class TrackerStorage
    {
        private readonly object synchronized;
        private readonly Dictionary<string, TrackerStorageEntry> entries;

        public TrackerStorage()
        {
            this.synchronized = new object();
            this.entries = new Dictionary<string, TrackerStorageEntry>();
        }

        public void Register(string tracker, Action<TrackerAnnounceConfiguration> announce)
        {
            lock (synchronized)
            {
                TrackerStorageEntry entry;
                entries.TryGetValue(tracker, out entry);

                if (entry == null)
                {
                    entry = new TrackerStorageEntry
                    {
                        Client = TrackerClientFactory.Create(tracker),
                        Announces = new List<Action<TrackerAnnounceConfiguration>>(),
                        Trigger = DateTime.Now
                    };

                    entries.Add(tracker, entry);
                }

                entry.Announces.Add(announce);
            }
        }

        public TrackerTelegraphRequest[] GetRequests()
        {
            List<TrackerTelegraphRequest> requests = new List<TrackerTelegraphRequest>();

            lock (synchronized)
            {
                foreach (TrackerStorageEntry entry in entries.Values)
                {
                    foreach (Action<TrackerAnnounceConfiguration> announce in entry.Announces)
                    {
                        requests.Add(new TrackerTelegraphRequest(entry.Client, announce, entry));
                    }
                }
            }

            return requests.ToArray();
        }
    }
}