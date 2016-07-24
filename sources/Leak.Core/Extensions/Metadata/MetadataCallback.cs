using Leak.Core.Common;

namespace Leak.Core.Extensions.Metadata
{
    public interface MetadataCallback
    {
        void OnRequest(PeerHash peer, MetadataRequest request);

        void OnData(PeerHash peer, MetadataData data);

        void OnReject(PeerHash peer, MetadataReject reject);
    }
}