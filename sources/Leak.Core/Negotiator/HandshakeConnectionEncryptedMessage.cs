using Leak.Common;
using Leak.Core.Network;

namespace Leak.Core.Negotiator
{
    public class HandshakeConnectionEncryptedMessage : NetworkOutgoingMessage
    {
        private readonly NetworkOutgoingMessage inner;
        private readonly HandshakeKey key;

        public HandshakeConnectionEncryptedMessage(NetworkOutgoingMessage inner, HandshakeKey key)
        {
            this.inner = inner;
            this.key = key;
        }

        public int Length
        {
            get { return inner.Length; }
        }

        public byte[] ToBytes()
        {
            return key.Encrypt(inner.ToBytes());
        }
    }
}