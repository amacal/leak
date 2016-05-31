using System.Collections.Generic;

namespace Leak.Core.Net
{
    public class PeerNegotiatorHashCollection
    {
        private readonly List<byte[]> items;

        public PeerNegotiatorHashCollection(params byte[][] items)
        {
            this.items = new List<byte[]>(items);
        }

        public byte[] Find(byte[] secret, byte[] hash)
        {
            foreach (byte[] item in items)
                if (Bytes.Equals(hash, PeerCryptoHash.GetXor(secret, item)))
                    return item;

            return null;
        }
    }
}