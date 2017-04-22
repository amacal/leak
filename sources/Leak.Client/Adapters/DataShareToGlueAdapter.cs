using Leak.Common;
using Leak.Data.Share;
using Leak.Networking.Core;
using Leak.Peer.Coordinator;

namespace Leak.Client.Adapters
{
    public class DataShareToGlueAdapter : DataShareToGlue
    {
        private readonly CoordinatorService service;

        public DataShareToGlueAdapter(CoordinatorService service)
        {
            this.service = service;
        }

        public void SendUnchoke(PeerHash peer)
        {
            service.Choke(peer, false);
        }

        public void SendPiece(PeerHash peer, BlockIndex block, DataBlock payload)
        {
            service.SendPiece(peer, block, payload);
        }
    }
}