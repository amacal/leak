using Leak.Core.Common;

namespace Leak.Core.Cando.Metadata
{
    public abstract class MetadataCallbackBase : MetadataCallback
    {
        public virtual void OnRequest(PeerSession session, MetadataRequest request)
        {
        }

        public virtual void OnData(PeerSession session, MetadataData data)
        {
        }

        public virtual void OnReject(PeerSession session, MetadataReject reject)
        {
        }
    }
}