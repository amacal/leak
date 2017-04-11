using Leak.Common;
using Leak.Data.Share;
using Leak.Networking.Core;
using Leak.Peer.Coordinator;

namespace Leak.Client.Adapters
{
    public class DataShareToGlueAdapter : DataShareToGlue
    {
        private readonly GlueService service;

        public DataShareToGlueAdapter(GlueService service)
        {
            this.service = service;
        }

        public void SendUnchoke(PeerHash peer)
        {
            service.SendUnchoke(peer);
        }

        public void SendPiece(PeerHash peer, BlockIndex block, DataBlock payload)
        {
            service.SendPiece(peer, block, payload);
        }
    }
}