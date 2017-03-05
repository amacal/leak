using System;
using Leak.Common;
using Leak.Tasks;

namespace Leak.Tracker.Get
{
    public class TrackerGetNextTask : LeakTask<TrackerGetContext>
    {
        public void Execute(TrackerGetContext context)
        {
            DateTime now = DateTime.Now;
            TrackerGetFactory factory = new TrackerGetFactory(context);

            foreach (TrackerGetEntry entry in context.Collection.Find(now))
            {
                TrackerGetProxy proxy = factory.Create(entry.Request.Address);
                Action<TimeSpan> callback = OnAnnounced(entry);

                entry.Next = now + TimeSpan.FromMinutes(15);
                proxy.Announce(entry.Request, callback);
            }
        }

        private Action<TimeSpan> OnAnnounced(TrackerGetEntry entry)
        {
            return interval =>
            {
                DateTime now = DateTime.Now;
                entry.Next = now + interval;
            };
        }
    }
}