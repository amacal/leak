using Leak.Core.Network;

namespace Leak.Core.Net
{
    public class PeerCryptoHash : PeerMessageFactory
    {
        private readonly byte[] hash;
        private readonly byte[] xor;

        public PeerCryptoHash(byte[] secret, byte[] hash)
        {
            this.hash = GetHash(secret);
            this.xor = GetXor(secret, hash);
        }

        public static byte[] GetHash(byte[] secret)
        {
            return Bytes.Hash("req1", secret);
        }

        public static byte[] GetXor(byte[] secret, byte[] hash)
        {
            return Bytes.Xor(Bytes.Hash("req2", hash), Bytes.Hash("req3", secret));
        }

        public byte[] Hash
        {
            get { return hash; }
        }

        public byte[] Xor
        {
            get { return xor; }
        }

        public override NetworkOutgoingMessage GetMessage()
        {
            byte[] payload = new byte[0];

            Bytes.Append(ref payload, hash);
            Bytes.Append(ref payload, xor);

            return new NetworkOutgoingMessage(payload);
        }
    }
}