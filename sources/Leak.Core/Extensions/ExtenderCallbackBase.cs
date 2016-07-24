using Leak.Core.Common;

namespace Leak.Core.Extensions
{
    public abstract class ExtenderCallbackBase : ExtenderCallback
    {
        public virtual void OnHandshake(PeerHash peer, ExtenderHandshake handshake)
        {
        }
    }
}