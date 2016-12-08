using Leak.Common;
using Leak.Core.Network;

namespace Leak.Core.Negotiator
{
    public class HandshakeKeyExchange
    {
        private readonly byte[] key;
        private readonly byte[] padding;

        public HandshakeKeyExchange(NetworkIncomingMessage message)
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
    }
}