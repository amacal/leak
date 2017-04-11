using Leak.Common;

namespace Leak.Peer.Negotiator
{
    public class HandshakeMatch
    {
        private readonly byte[] secret;
        private readonly byte[] data;

        public HandshakeMatch(byte[] secret, byte[] data)
        {
            this.secret = secret;
            this.data = data;
        }

        public bool Matches(FileHash hash)
        {
            return Bytes.Equals(data, HandshakeCryptoHashMessage.GetXor(secret, hash.ToBytes()));
        }
    }
}