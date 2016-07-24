using Leak.Core.Network;

namespace Leak.Core.Negotiator
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

        public byte[] ToBytes()
        {
            return Bytes.Concatenate(credentials.PublicKey, credentials.Padding);
        }
    }
}