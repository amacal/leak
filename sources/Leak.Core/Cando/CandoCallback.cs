using Leak.Common;
using Leak.Communicator.Messages;

namespace Leak.Core.Cando
{
    public interface CandoCallback
    {
        void OnOutgoingMessage(PeerSession session, ExtendedOutgoingMessage message);
    }
}