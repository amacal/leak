using Leak.Core.Common;

namespace Leak.Core.Extensions
{
    public interface ExtenderCallback
    {
        void OnHandshake(PeerHash peer, ExtenderHandshake handshake);
    }
}