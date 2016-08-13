using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Cando
{
    public interface CandoCallback
    {
        void OnOutgoingMessage(PeerHash peer, ExtendedOutgoingMessage message);
    }
}