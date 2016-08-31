using Leak.Core.Tracker;
using System;

namespace Leak.Core.Telegraph
{
    public abstract class TelegraphCallbackBase : TelegraphCallback
    {
        public virtual void OnAnnouncingStarted(TrackerRequest configuration)
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