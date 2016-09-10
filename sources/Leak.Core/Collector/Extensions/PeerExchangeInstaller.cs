using Leak.Core.Cando;
using Leak.Core.Collector.Callbacks;

namespace Leak.Core.Collector.Extensions
{
    public class PeerExchangeInstaller : PeerCollectorExtension
    {
        private readonly PeerCollectorContext context;

        public PeerExchangeInstaller(PeerCollectorContext context)
        {
            this.context = context;
        }

        public void Install(CandoConfiguration cando)
        {
            cando.Extensions.PeerExchange(with =>
            {
                with.Callback = new PeerCollectorToExchange(context);
            });
        }
    }
}