namespace Leak.Core.Net
{
    public class PeerUnchoke : PeerMessageFactory
    {
        public override PeerMessage GetMessage()
        {
            return new PeerMessage(0x00, 0x00, 0x00, 0x01, 0x01);
        }
    }
}