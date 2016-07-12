using Leak.Core.Network;

namespace Leak.Core.Net
{
    public class PeerCryptoPayload : PeerMessageFactory
    {
        public static readonly int MinimumSize = 14;

        private readonly int options;
        private readonly byte[] verification;
        private readonly byte[] padding;

        public PeerCryptoPayload()
        {
            this.options = 2;
            this.verification = GetVerification();
            this.padding = new byte[0];
        }

        public PeerCryptoPayload(NetworkIncomingMessage message)
        {
            this.options = message[11];

            this.verification = message.ToBytes(0, 8);
            this.padding = message.ToBytes(14, GetPaddingSize(message));
        }

        public byte[] Verification
        {
            get { return verification; }
        }

        public static int GetSize(NetworkIncomingMessage message)
        {
            return 14 + GetPaddingSize(message);
        }

        public static byte[] GetVerification()
        {
            return Bytes.Parse("0000000000000000");
        }

        private static int GetPaddingSize(NetworkIncomingMessage message)
        {
            return message[12] * 256 + message[13];
        }

        public override NetworkOutgoingMessage GetMessage()
        {
            byte[] payload = new byte[0];

            Bytes.Append(ref payload, verification);
            Bytes.Append(ref payload, Bytes.Parse("00000002"));
            Bytes.Append(ref payload, Bytes.Parse("0000"));

            return new NetworkOutgoingMessage(payload);
        }
    }
}