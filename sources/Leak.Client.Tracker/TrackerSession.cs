using System.Threading.Tasks;
using Leak.Common;

namespace Leak.Client.Tracker
{
    public class TrackerSession : Session
    {
        private readonly TrackerConnect inner;

        internal TrackerSession(TrackerConnect inner)
        {
            this.inner = inner;
        }

        public void Announce(params FileHash[] hashes)
        {
            inner.Announce(hashes);
        }

        public Task<Notification> NextAsync()
        {
            return inner.Notifications.NextAsync();
        }
    }
}