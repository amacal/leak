using Leak.Core.Metadata;

namespace Leak.Core.Client
{
    public abstract class PeerClientCallbackBase : PeerClientCallback
    {
        public virtual void OnStarted(Metainfo metainfo)
        {
        }

        public virtual void OnCompleted(Metainfo metainfo)
        {
        }
    }
}