using Leak.Core.Network;

namespace Leak.Core.Net
{
    public class PeerInterested : PeerMessageFactory
    {
        public override NetworkOutgoingMessageBytes GetMessage()
        {
            return new NetworkOutgoingMessageBytes(0x00, 0x00, 0x00, 0x01, 0x02);
        }
    }
}