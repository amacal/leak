using Leak.Common;
using Leak.Dataget;
using Leak.Glue;

namespace Leak.Leakage
{
    public static class LeakGlueExtensions
    {
        public static RetrieverGlue ToRetriever(this GlueService service)
        {
            return new GlueToRetriever(service);
        }

        private class GlueToRetriever : RetrieverGlue
        {
            private readonly GlueService service;

            public GlueToRetriever(GlueService service)
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
}