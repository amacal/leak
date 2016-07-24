using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Extensions
{
    public interface ExtenderHandler
    {
        bool CanHandle(string name);

        void Handle(PeerHash peer, ExtendedIncomingMessage message);
    }
}