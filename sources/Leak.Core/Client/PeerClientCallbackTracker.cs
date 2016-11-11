using Leak.Core.Common;

namespace Leak.Core.Client
{
    public interface PeerClientCallbackTracker
    {
        void OnAnnounceStarted(FileHash hash);

        void OnAnnounceFailed(FileHash hash);
    }
}