using Leak.Core.Common;

namespace Leak.Core.Cando.Metadata
{
    public abstract class MetadataCallbackBase : MetadataCallback
    {
        public virtual void OnSize(PeerHash peer, MetadataSize size)
        {
        }

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