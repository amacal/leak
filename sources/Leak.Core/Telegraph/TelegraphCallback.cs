using Leak.Core.Tracker;
using System;

namespace Leak.Core.Telegraph
{
    public interface TelegraphCallback
    {
        void OnAnnouncingStarted(TrackerRequest configuration);

        void OnAnnouncingCompleted(TrackerAnnounce announce);

        void OnException(Exception ex);
    }
}