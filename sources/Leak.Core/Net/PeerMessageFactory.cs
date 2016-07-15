using Leak.Core.Network;

namespace Leak.Core.Net
{
    public abstract class PeerMessageFactory
    {
        public abstract NetworkOutgoingMessageBytes GetMessage();
    }
}