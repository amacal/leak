using Leak.Common;

namespace Leak.Retriever
{
    public interface RetrieverGlue
    {
        void SendInterested(PeerHash peer);

        void SendRequest(PeerHash peer, BlockIndex block);
    }
}
