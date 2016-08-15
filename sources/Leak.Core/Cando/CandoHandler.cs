using Leak.Core.Bencoding;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Cando
{
    public interface CandoHandler
    {
        bool CanHandle(string name);

        void OnHandshake(PeerHash peer, BencodedValue handshake);

        void OnMessage(PeerHash peer, Extended payload);
    }
}