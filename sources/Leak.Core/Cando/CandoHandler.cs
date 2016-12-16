using Leak.Common;
using Leak.Communicator.Messages;
using Leak.Core.Bencoding;

namespace Leak.Core.Cando
{
    public interface CandoHandler
    {
        bool CanHandle(string name);

        void OnHandshake(PeerSession session, BencodedValue handshake);

        void OnMessage(PeerSession session, Extended payload);
    }
}