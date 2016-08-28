using Leak.Core.Tracker;
using System;

namespace Leak.Core.Telegraph
{
    public interface TrackerTelegraphCallback
    {
        void OnAnnouncingStarted(TrackerAnnounceConfiguration configuration);

        void OnAnnouncingCompleted(TrackerAnnounce announce);

        void OnException(Exception ex);
    }
}