using Leak.Core.Bencoding;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Cando
{
    public interface CandoHandler
    {
        bool CanHandle(string name);

        void OnHandshake(PeerSession session, BencodedValue handshake);

        void OnMessage(PeerSession session, Extended payload);
    }
}