using Leak.Core.Net;
using Leak.Core.Network;

namespace Leak.Core.Negotiator
{
    public class HandshakeKeyExchangeMessage : NetworkOutgoingMessage
    {
        private readonly PeerCredentials credentials;

        public HandshakeKeyExchangeMessage(PeerCredentials credentials)
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