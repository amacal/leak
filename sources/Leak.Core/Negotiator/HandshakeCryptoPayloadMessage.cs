using Leak.Common;

namespace Leak.Core.Negotiator
{
    public class HandshakeCryptoPayloadMessage : NetworkOutgoingMessage
    {
        private readonly int method;

        public HandshakeCryptoPayloadMessage(int method)
        {
            this.method = method;
        }

        public int Length
        {
            get { return 14; }
        }

        public byte[] ToBytes()
        {
            return Bytes.Concatenate(HandshakeCryptoPayload.GetVerification(), Bytes.ToInt32(method), Bytes.Parse("0000"));
        }
    }
}