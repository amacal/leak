using Leak.Core.Common;

namespace Leak.Core.Extensions.Metadata
{
    public abstract class MetadataCallbackBase : MetadataCallback
    {
        public virtual void OnRequest(PeerHash peer, MetadataRequest request)
        {
        }

        public virtual void OnData(PeerHash peer, MetadataData data)
        {
        }

        public virtual void OnReject(PeerHash peer, MetadataReject reject)
        {
        }
    }
}