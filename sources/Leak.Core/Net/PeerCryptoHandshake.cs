using Leak.Core.Network;

namespace Leak.Core.Net
{
    public class PeerCryptoHandshake : PeerMessageFactory
    {
        public static readonly int MinimumSize = 2;

        public static int GetSize(NetworkIncomingMessage message)
        {
            return 2 + message[0] * 256 + message[1];
        }

        public override NetworkOutgoingMessage GetMessage()
        {
            return new NetworkOutgoingMessage(Bytes.Parse("0000"));
        }
    }
}