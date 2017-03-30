using Leak.Common;

namespace Leak.Data.Share
{
    public interface DataShareToGlue
    {
        void SendUnchoke(PeerHash peer);

        void SendPiece(PeerHash peer, BlockIndex block, DataBlock payload);
    }
}