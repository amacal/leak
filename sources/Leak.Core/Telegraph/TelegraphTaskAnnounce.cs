using Leak.Core.Core;
using Leak.Core.Tracker;
using System;

namespace Leak.Core.Telegraph
{
    public class TelegraphTaskAnnounce : LeakTask<TelegraphContext>
    {
        private readonly TelegraphEntry entry;

        public TelegraphTaskAnnounce(TelegraphEntry entry)
        {
            this.entry = entry;
        }

        public void Execute(TelegraphContext context)
        {
            entry.Next = DateTime.Now.AddMinutes(30);

            try
            {
                TrackerClient client = TrackerClientFactory.Create(entry.Tracker);
                TrackerAnnounce response = client.Announce(entry.Request);

                context.Callback.OnAnnouncingCompleted(response);
                entry.Next = DateTime.Now.Add(response.Interval);
            }
            catch (Exception ex)
            {
                context.Callback.OnException(ex);
            }
        }
    }
}