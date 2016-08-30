using Leak.Core.Client.Events;
using Leak.Core.Common;

namespace Leak.Core.Client
{
    public interface PeerClientCallbackTracker
    {
        void OnAnnounceStarted(FileHash hash);

        void OnAnnounceCompleted(FileHash hash, FileAnnouncedEvent @event);

        void OnAnnounceFailed(FileHash hash);
    }
}