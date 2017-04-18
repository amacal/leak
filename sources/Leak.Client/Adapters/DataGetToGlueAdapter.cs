using Leak.Common;
using Leak.Data.Get;
using Leak.Peer.Coordinator;

namespace Leak.Client.Adapters
{
    internal class DataGetToGlueAdapter : DataGetToGlue
    {
        private readonly CoordinatorService service;

        public DataGetToGlueAdapter(CoordinatorService service)
        {
            this.service = service;
        }

        public void SendInterested(PeerHash peer)
        {
            service.Interested(peer);
        }

        public void SendRequest(PeerHash peer, BlockIndex block)
        {
            service.SendRequest(peer, block);
        }
    }
}