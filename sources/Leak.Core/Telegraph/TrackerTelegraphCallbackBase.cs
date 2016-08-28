using Leak.Core.Tracker;
using System;

namespace Leak.Core.Telegraph
{
    public abstract class TrackerTelegraphCallbackBase : TrackerTelegraphCallback
    {
        public virtual void OnAnnouncingStarted(TrackerAnnounceConfiguration configuration)
        {
        }

        public virtual void OnAnnouncingCompleted(TrackerAnnounce announce)
        {
        }

        public virtual void OnException(Exception ex)
        {
        }
    }
}