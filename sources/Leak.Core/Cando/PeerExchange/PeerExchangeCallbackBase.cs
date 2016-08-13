using Leak.Core.Common;

namespace Leak.Core.Cando.PeerExchange
{
    public abstract class PeerExchangeCallbackBase : PeerExchangeCallback
    {
        public virtual void OnMessage(PeerHash peer, PeerExchangeMessage message)
        {
        }
    }
}