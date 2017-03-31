using Leak.Common;

namespace Leak.Negotiator
{
    public class HandshakeKeyExchangeMessage : NetworkOutgoingMessage
    {
        private readonly HandshakeCredentials credentials;

        public HandshakeKeyExchangeMessage(HandshakeCredentials credentials)
        {
            this.credentials = credentials;
        }

        public int Length
        {
            get { return credentials.PublicKey.Length + credentials.Padding.Length; }
        }

        public DataBlock ToBytes(DataBlockFactory factory)
        {
            return factory.Transcient(Bytes.Concatenate(credentials.PublicKey, credentials.Padding), 0, Length);
        }
    }
}