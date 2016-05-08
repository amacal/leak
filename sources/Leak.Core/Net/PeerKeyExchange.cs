using System;

namespace Leak.Core.Net
{
    public class PeerKeyExchange : PeerMessageFactory
    {
        private readonly byte[] key;
        private readonly byte[] padding;

        public PeerKeyExchange(PeerCredentials credentials)
        {
            this.key = credentials.PublicKey;
            this.padding = credentials.Padding;
        }

        public PeerKeyExchange(PeerMessage message)
        {
            this.key = message.ToBytes(0, 96);
            this.padding = message.ToBytes(96);
        }

        public byte[] Key
        {
            get { return key; }
        }

        public byte[] Padding
        {
            get { return padding; }
        }

        public override PeerMessage GetMessage()
        {
            byte[] data = new byte[key.Length + padding.Length];

            Array.Copy(key, 0, data, 0, key.Length);
            Array.Copy(padding, 0, data, key.Length, padding.Length);

            return new PeerMessage(data);
        }
    }
}