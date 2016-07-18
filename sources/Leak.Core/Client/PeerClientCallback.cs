using Leak.Core.Metadata;

namespace Leak.Core.Client
{
    public interface PeerClientCallback
    {
        void OnStarted(Metainfo metainfo);

        void OnCompleted(Metainfo metainfo);
    }
}