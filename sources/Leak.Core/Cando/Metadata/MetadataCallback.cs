using Leak.Core.Common;

namespace Leak.Core.Cando.Metadata
{
    public interface MetadataCallback
    {
        void OnSize(PeerHash peer, MetadataSize size);

        void OnRequest(PeerHash peer, MetadataRequest request);

        void OnData(PeerHash peer, MetadataData data);

        void OnReject(PeerHash peer, MetadataReject reject);
    }
}