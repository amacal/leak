using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Data.Share
{
    public interface DataShareToGlue
    {
        void SendUnchoke(PeerHash peer);

        void SendPiece(PeerHash peer, BlockIndex block, DataBlock payload);
    }
}