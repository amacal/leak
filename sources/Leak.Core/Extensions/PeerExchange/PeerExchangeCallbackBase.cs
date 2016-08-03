using Leak.Core.Common;

namespace Leak.Core.Extensions.PeerExchange
{
    public abstract class PeerExchangeCallbackBase : PeerExchangeCallback
    {
        public virtual void OnMessage(PeerHash peer, PeerExchangeMessage message)
        {
        }
    }
}