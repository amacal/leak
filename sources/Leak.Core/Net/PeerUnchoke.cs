using Leak.Core.Network;

namespace Leak.Core.Net
{
    public class PeerUnchoke : PeerMessageFactory
    {
        public override NetworkOutgoingMessage GetMessage()
        {
            return new NetworkOutgoingMessage(0x00, 0x00, 0x00, 0x01, 0x01);
        }
    }
}