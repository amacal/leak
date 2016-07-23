using Leak.Core.Tracker;
using System;
using System.Collections.Generic;

namespace Leak.Core.Telegraph
{
    public class TrackerStorageEntry
    {
        public TrackerClient Client { get; set; }

        public List<Action<TrackerAnnounceConfiguration>> Announces { get; set; }

        public DateTime Trigger { get; set; }
    }
}