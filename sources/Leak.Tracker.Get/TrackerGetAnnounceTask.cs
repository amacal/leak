using System;
using Leak.Common;
using Leak.Tasks;

namespace Leak.Tracker.Get
{
    public class TrackerGetAnnounceTask : LeakTask<TrackerGetContext>
    {
        private readonly FileHash hash;

        public TrackerGetAnnounceTask(FileHash hash)
        {
            this.hash = hash;
        }

        public void Execute(TrackerGetContext context)
        {
            DateTime now = DateTime.Now;
            TrackerGetFactory factory = new TrackerGetFactory(context);

            foreach (TrackerGetEntry entry in context.Collection.Find(hash))
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