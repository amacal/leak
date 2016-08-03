using Leak.Core.Common;

namespace Leak.Core.Extensions.PeerExchange
{
    public interface PeerExchangeCallback
    {
        void OnMessage(PeerHash peer, PeerExchangeMessage message);
    }
}