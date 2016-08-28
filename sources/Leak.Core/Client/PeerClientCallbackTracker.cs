using Leak.Core.Common;

namespace Leak.Core.Client
{
    public interface PeerClientCallbackTracker
    {
        void OnAnnounceStarted(FileHash hash);

        void OnAnnounceCompleted(FileHash hash, PeerClientAnnounced announced);

        void OnAnnounceFailed(FileHash hash);
    }
}