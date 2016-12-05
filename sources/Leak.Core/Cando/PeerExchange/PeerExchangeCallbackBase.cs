using Leak.Common;

namespace Leak.Core.Cando.PeerExchange
{
    public abstract class PeerExchangeCallbackBase : PeerExchangeCallback
    {
        public virtual void OnMessage(PeerSession session, PeerExchangeData data)
        {
        }
    }
}