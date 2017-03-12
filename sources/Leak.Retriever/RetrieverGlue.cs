using Leak.Common;

namespace Leak.Data.Get
{
    public interface RetrieverGlue
    {
        void SendInterested(PeerHash peer);

        void SendRequest(PeerHash peer, BlockIndex block);
    }
}