using Leak.Core.Network;

namespace Leak.Core.Negotiator
{
    public class HandshakeCryptoPayloadMessage : NetworkOutgoingMessage
    {
        public int Length
        {
            get { return 14; }
        }

        public byte[] ToBytes()
        {
            return Bytes.Concatenate(HandshakeCryptoPayload.GetVerification(), Bytes.Parse("00000002"), Bytes.Parse("0000"));
        }
    }
}