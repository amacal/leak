using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Cando
{
    public interface CandoHandler
    {
        bool CanHandle(string name);

        void Handle(PeerHash peer, Extended payload);
    }
}