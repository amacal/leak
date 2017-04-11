using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
{
    public class HandshakeCryptoPayload
    {
        public static readonly int MinimumSize = 14;

        public static byte[] GetVerification()
        {
            return Bytes.Parse("0000000000000000");
        }

        public static int GetMethod(NetworkIncomingMessage message)
        {
            return message[11];
        }

        public static int GetSize(NetworkIncomingMessage message)
        {
            return 14 + GetPaddingSize(message);
        }

        private static int GetPaddingSize(NetworkIncomingMessage message)
        {
            return message[12] * 256 + message[13];
        }
    }
}