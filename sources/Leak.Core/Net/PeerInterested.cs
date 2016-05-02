namespace Leak.Core.Net
{
    public class PeerInterested : PeerMessageFactory
    {
        public override PeerMessage GetMessage()
        {
            return new PeerMessage(0x00, 0x00, 0x00, 0x01, 0x02);
        }
    }
}