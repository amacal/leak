using Leak.Common;
using Leak.Data.Get;
using Leak.Glue;

namespace Leak.Client.Adapters
{
    internal class DataGetToGlueAdapter : DataGetToGlue
    {
        private readonly GlueService service;

        public DataGetToGlueAdapter(GlueService service)
        {
            this.service = service;
        }

        public void SendInterested(PeerHash peer)
        {
            service.SendInterested(peer);
        }

        public void SendRequest(PeerHash peer, BlockIndex block)
        {
            service.SendRequest(peer, block);
        }
    }
}