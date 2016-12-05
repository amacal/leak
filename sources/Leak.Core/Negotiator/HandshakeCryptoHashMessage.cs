using Leak.Common;
using Leak.Core.Network;

namespace Leak.Core.Negotiator
{
    public class HandshakeCryptoHashMessage : NetworkOutgoingMessage
    {
        private readonly byte[] hash;
        private readonly byte[] xor;

        public HandshakeCryptoHashMessage(byte[] secret, byte[] hash)
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

        public int Length
        {
            get { return hash.Length + xor.Length; }
        }

        public byte[] ToBytes()
        {
            return Bytes.Concatenate(hash, xor);
        }
    }
}