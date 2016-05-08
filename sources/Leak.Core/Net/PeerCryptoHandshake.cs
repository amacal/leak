namespace Leak.Core.Net
{
    public class PeerCryptoHandshake : PeerMessageFactory
    {
        public static readonly int MinimumSize = 2;

        public static int GetSize(PeerMessage message)
        {
            return 2 + message[0] * 256 + message[1];
        }

        public override PeerMessage GetMessage()
        {
            return new PeerMessage(Bytes.Parse("0000"));
        }
    }
}