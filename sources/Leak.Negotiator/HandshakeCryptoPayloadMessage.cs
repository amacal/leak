using Leak.Common;

namespace Leak.Negotiator
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

        public DataBlock ToBytes(DataBlockFactory factory)
        {
            return factory.Transcient(Bytes.Concatenate(HandshakeCryptoPayload.GetVerification(), Bytes.ToInt32(method), Bytes.Parse("0000")), 0, Length);
        }
    }
}