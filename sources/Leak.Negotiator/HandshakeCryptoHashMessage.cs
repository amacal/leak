using System;
using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
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

        public void ToBytes(DataBlock block)
        {
            block.With((buffer, offset, count) =>
            {
                Array.Copy(Bytes.Concatenate(hash, xor), 0, buffer, offset, Length);
            });
        }

        public void Release()
        {
        }
    }
}