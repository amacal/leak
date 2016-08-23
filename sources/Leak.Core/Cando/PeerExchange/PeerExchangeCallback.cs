using Leak.Core.Common;

namespace Leak.Core.Cando.PeerExchange
{
    public interface PeerExchangeCallback
    {
        void OnMessage(PeerSession session, PeerExchangeData data);
    }
}