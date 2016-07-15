namespace Leak.Core.Negotiator
{
    public class HandshakeHashMatch
    {
        private readonly byte[] secret;
        private readonly byte[] data;

        public HandshakeHashMatch(byte[] secret, byte[] data)
        {
            this.secret = secret;
            this.data = data;
        }

        public bool Matches(HandshakeHash hash)
        {
            return Bytes.Equals(data, HandshakeCryptoHashMessage.GetXor(secret, hash.ToBytes()));
        }
    }
}