using Leak.Core.Tracker;
using System;

namespace Leak.Core.Telegraph
{
    public interface TrackerTelegraphCallback
    {
        void OnAnnounced(TrackerAnnounce announce);

        void OnException(Exception ex);
    }
}