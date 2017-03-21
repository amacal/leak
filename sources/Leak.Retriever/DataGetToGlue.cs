using Leak.Common;

namespace Leak.Data.Get
{
    public interface DataGetToGlue
    {
        void SendInterested(PeerHash peer);

        void SendRequest(PeerHash peer, BlockIndex block);
    }
}