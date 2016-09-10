using Leak.Core.Collector;

namespace Leak.Core.Client.Configuration
{
    public class PeerClientPeerExchangeBuilder
    {
        private PeerClientPeerExchangeStatus status;

        public PeerClientPeerExchangeBuilder()
        {
            status = PeerClientPeerExchangeStatus.Off;
        }

        public void Disable()
        {
            status = PeerClientPeerExchangeStatus.Off;
        }

        public void Enable()
        {
            status = PeerClientPeerExchangeStatus.On;
        }

        public void Apply(PeerCollectorConfiguration configuration)
        {
            if (status == PeerClientPeerExchangeStatus.On)
            {
                configuration.Extensions.IncludePeerExchange();
            }
        }
    }
}