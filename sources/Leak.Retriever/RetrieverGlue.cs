using Leak.Common;

namespace Leak.Dataget
{
    public interface RetrieverGlue
    {
        void SendInterested(PeerHash peer);

        void SendRequest(PeerHash peer, BlockIndex block);
    }
}