using System;
using Leak.Events;

namespace Leak.Tracker.Get
{
    public class TrackerGetHooks
    {
        public Action<TrackerAnnounced> OnAnnounced;

        public Action<TrackerFailed> OnFailed;
    }
}