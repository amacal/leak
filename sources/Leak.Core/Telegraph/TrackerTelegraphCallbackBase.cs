using Leak.Core.Tracker;
using System;

namespace Leak.Core.Telegraph
{
    public abstract class TrackerTelegraphCallbackBase : TrackerTelegraphCallback
    {
        public virtual void OnAnnounced(TrackerAnnounce announce)
        {
        }

        public virtual void OnException(Exception ex)
        {
        }
    }
}