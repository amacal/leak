using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Cando
{
    public abstract class CandoCallbackBase : CandoCallback
    {
        public virtual void OnOutgoingMessage(PeerHash peer, ExtendedOutgoingMessage message)
        {
        }
    }
}