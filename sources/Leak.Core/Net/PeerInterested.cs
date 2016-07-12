using Leak.Core.Network;

namespace Leak.Core.Net
{
    public class PeerInterested : PeerMessageFactory
    {
        public override NetworkOutgoingMessage GetMessage()
        {
            return new NetworkOutgoingMessage(0x00, 0x00, 0x00, 0x01, 0x02);
        }
    }
}