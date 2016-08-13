using Leak.Core.Cando.PeerExchange;

namespace Leak.Core.Collector.Callbacks
{
    public class PeerCollectorExchange : PeerExchangeCallbackBase
    {
        private readonly PeerCollectorContext context;

        public PeerCollectorExchange(PeerCollectorContext context)
        {
            this.context = context;
        }
    }
}